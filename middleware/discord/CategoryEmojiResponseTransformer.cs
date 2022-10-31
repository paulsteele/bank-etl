using System.Data;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord;

public class CategoryEmojiResponseTransformer : ICategoryTransformer
{
	private readonly DiscordClient _client;
	private readonly ILogger<CategoryEmojiResponseTransformer> _logger;

	public CategoryEmojiResponseTransformer(
		DiscordClient client, 
		ILogger<CategoryEmojiResponseTransformer> logger
	)
	{
		_client = client;
		_logger = logger;
	}
	public string SourceState => "WaitingForEmoji";
	private const string ReceivedFromFirefly = nameof(ReceivedFromFirefly);
	public string Setup => nameof(Setup);
	public async Task Transform(Category item, IDb db)
	{
		if (!item.DiscordMessageId.HasValue)
		{
			_logger.LogError($"{item.Name} has no discord message");
			return;
		}
		var reactions = (await _client.GetReactions(item.DiscordMessageId.Value)).ToArray();

		if (reactions.Length == 1)
		{
			var existing = db.GetAllCategories().Where(c => c.Emoji == reactions[0].Name);

			if (existing.Any())
			{
				var newId = await _client.SendMessage($"The emoji for {item.Name} was already selected. Try again");
				if (newId == 0)
				{
					_logger.LogError("Could not send reselection message");
					return;
				}

				item.DiscordMessageId = newId;
				
				return;
			}
			
			
			item.Emoji = reactions[0].Name;
			item.State = Setup;

			if (item.DiscordMessageId.HasValue)
			{
				await _client.React(item.DiscordMessageId.Value, new[] {"✅"});
			}
			
			_logger.LogInformation($"Setup {item.Name} with emoji {item.Emoji}");
		}
	}
}