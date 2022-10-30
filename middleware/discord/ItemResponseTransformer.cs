using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord;

public class ItemResponseTransformer : IBankItemTransformer
{
	private readonly DiscordClient _client;
	private readonly ILogger<ItemResponseTransformer> _logger;
	public string SourceState => "WaitingForEmoji";
	public string DestinationState => "ReadyForUpload";
	
	public ItemResponseTransformer(
		DiscordClient client,
		ILogger<ItemResponseTransformer> logger
	)
	{
		_client = client;
		_logger = logger;
	}

	public async Task Transform(BankItem item, IDb db)
	{
		if (!item.DiscordMessageId.HasValue)
		{
			_logger.LogError($"{item.Id} does not have a {nameof(BankItem.DiscordMessageId)}");
			return;
		}

		var reactions = (await _client.GetReactions(item.DiscordMessageId.Value))
			.Where(items => items.ReactionCount > 1)
			.ToArray();

		if (reactions.Length != 1)
		{
			return;
		}

		var category = db.GetAllCategories().FirstOrDefault(c => c.Emoji == reactions.First().Name);

		if (category == null)
		{
			_logger.LogError($"{item.Id} had an unfound emoji");
			return;
		}
		
		item.Category = category;
		item.State = DestinationState;
		_logger.LogInformation($"{item.Id} set to {category.Name}");
	}
}