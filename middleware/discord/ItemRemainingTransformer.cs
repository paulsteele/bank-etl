using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord;

public class ItemRemainingTransformer : IBankItemTransformer
{
	private readonly DiscordClient _client;
	private readonly ILogger<ItemRemainingTransformer> _logger;
	public string SourceState => "SentToFirefly";
	public string DestinationState => "RemainingSent";
	
	public ItemRemainingTransformer(
		DiscordClient client,
		ILogger<ItemRemainingTransformer> logger
	)
	{
		_client = client;
		_logger = logger;
	}

	public async Task Transform(BankItem item, IDb db)
	{
		db.LoadForeignKeys(item);
		if (!item.DiscordMessageId.HasValue)
		{
			_logger.LogError($"{item.Id} does not have a {nameof(BankItem.DiscordMessageId)}");
			return;
		}

		if (item.Category == null)
		{
			_logger.LogError($"{item.Id} does not have a {nameof(BankItem.Category)}");
			return;
		}

		await _client.SendMessage($"Balance for {item.Category.Name}:\n${item.AmountInCategoryAfter:F2}");
		
		await _client.React(item.DiscordMessageId.Value, new[] {"✅"});
		item.State = DestinationState;
	}
}