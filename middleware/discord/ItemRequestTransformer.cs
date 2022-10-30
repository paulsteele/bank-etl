using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord;

public class ItemRequestTransformer : IBankItemTransformer
{
	private readonly DiscordClient _client;
	private readonly ILogger<ItemRequestTransformer> _logger;
	public string SourceState => "ParsedFromChase";
	public string DestinationState => "WaitingForEmoji";

	public ItemRequestTransformer(
		DiscordClient client,
		ILogger<ItemRequestTransformer> logger
	)
	{
		_client = client;
		_logger = logger;
	}

	public async Task Transform(BankItem item, IDb db)
	{
		var categories = db.GetAllCategories();
		var pendingCategories = categories.Any(c => !c.State.Equals("Setup"));
		if (pendingCategories || !categories.Any())
		{
			_logger.LogInformation("Skipping transformation due to pending categories");
			return;
		}

		var message = await _client.SendMessage($"${item.Amount:F2}\n{item.Vendor}\n{item.Timestamp}");

		if (message == 0)
		{
			_logger.LogError($"Could not send message for {item.Id}");
			return;
		}

		var result = await _client.React(message, categories.Select(c => c.Emoji).ToArray());
		
		if (!result)
		{
			_logger.LogError($"Could not react to message {item.Id}");
			return;
		}

		item.State = DestinationState;
		item.DiscordMessageId = message;
	}
}