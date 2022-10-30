using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord;

public class CategoryEmojiRequestTransformer : ICategoryTransformer
{
	private readonly DiscordClient _client;
	private readonly ILogger<CategoryEmojiRequestTransformer> _logger;

	public CategoryEmojiRequestTransformer(
		DiscordClient client, 
		ILogger<CategoryEmojiRequestTransformer> logger
	)
	{
		_client = client;
		_logger = logger;
	}
	public string SourceState => "ReceivedFromFirefly";
	public string WaitingForEmoji => nameof(WaitingForEmoji);
	public async Task Transform(Category item, IDb _)
	{
		var messageId = await _client.SendMessage($"React with the emoji for {item.Name}");
		if (messageId != 0)
		{
			item.State = WaitingForEmoji;
			item.DiscordMessageId = messageId;
			_logger.LogInformation($"Sent Emoji Request for {item.Name}");
		}
		else
		{
			_logger.LogError($"Could not send message for {item.Name}");
		}
	}
}