using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord.transformers;

public class CategoryEmojiRequestTransformer : ITransformer<Category>
{
	private readonly DiscordClient _client;
	private readonly ILogger<CategoryEmojiRequestTransformer> _logger;
	private readonly ErrorHandler _errorHandler;

	public CategoryEmojiRequestTransformer(
		DiscordClient client, 
		ILogger<CategoryEmojiRequestTransformer> logger,
		ErrorHandler errorHandler
	)
	{
		_client = client;
		_logger = logger;
		_errorHandler = errorHandler;
	}
	public Task<TransformResult<Category>> Transform(Category item, IDb _)
	{
		return _errorHandler.ExecuteWithErrorCatching(_logger, async () =>
		{
			var messageId = await _client.SendMessage($"React with the emoji for {item.Name}");
			if (messageId != 0)
			{
				item.DiscordMessageId = messageId;
				_logger.LogInformation($"Sent Emoji Request for {item.Name}");
			}
			else
			{
				_logger.LogError($"Could not send message for {item.Name}");
				return item.DefaultFailureResult();
			}

			return item.ToSuccessResult(TimeSpan.FromSeconds(30));
		},
			item.DefaultFailureResult()
		);
	}
}