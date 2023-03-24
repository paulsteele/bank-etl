using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord.transformers;

public class CategoryEmojiResponseTransformer : ITransformer<Category>
{
	private readonly DiscordClient _client;
	private readonly ILogger<CategoryEmojiResponseTransformer> _logger;
	private readonly ErrorHandler _errorHandler;

	public CategoryEmojiResponseTransformer(
		DiscordClient client, 
		ILogger<CategoryEmojiResponseTransformer> logger,
		ErrorHandler errorHandler
	)
	{
		_client = client;
		_logger = logger;
		_errorHandler = errorHandler;
	}
	public Task<TransformResult<Category>> Transform(Category item, IDb db)
	{
		return _errorHandler.ExecuteWithErrorCatching(_logger, async () =>
		{

			if (!item.DiscordMessageId.HasValue)
			{
				_logger.LogError($"{item.Name} has no discord message");
				return item.DefaultFailureResult();
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
						return item.DefaultFailureResult();
					}

					item.DiscordMessageId = newId;

					return item.DefaultFailureResult();
				}


				item.Emoji = reactions[0].Name;

				if (item.DiscordMessageId.HasValue)
				{
					await _client.React(item.DiscordMessageId.Value, new[] { "✅" });
				}

				_logger.LogInformation($"Setup {item.Name} with emoji {item.Emoji}");
			}

			return item.ToSuccessResult(TimeSpan.FromSeconds(30));
		},
			item.DefaultFailureResult()
		);
	}
}