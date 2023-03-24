using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord.transformers;

public class ItemResponseTransformer : ITransformer<BankItem>
{
	private readonly DiscordClient _client;
	private readonly ILogger<ItemResponseTransformer> _logger;
	private readonly ErrorHandler _errorHandler;

	public ItemResponseTransformer(
		DiscordClient client,
		ILogger<ItemResponseTransformer> logger,
		ErrorHandler errorHandler
	)
	{
		_client = client;
		_logger = logger;
		_errorHandler = errorHandler;
	}

	public Task<TransformResult<BankItem>> Transform(BankItem item, IDb db)
	{
		return _errorHandler.ExecuteWithErrorCatching(_logger, async () =>
		{
			if (!item.DiscordMessageId.HasValue)
			{
				_logger.LogError($"{item.Id} does not have a {nameof(BankItem.DiscordMessageId)}");
				return item.DefaultFailureResult();
			}

			var reactions = (await _client.GetReactions(item.DiscordMessageId.Value))
				.Where(items => items.ReactionCount > 1)
				.ToArray();

			if (reactions.Length != 1)
			{
				return item.DefaultFailureResult();
			}

			var category = db.GetAllCategories().FirstOrDefault(c => c.Emoji == reactions.First().Name);

			if (category == null)
			{
				_logger.LogError($"{item.Id} had an unfound emoji");
				return item.DefaultFailureResult();
			}

			item.Category = category;
			_logger.LogInformation($"{item.Id} set to {category.Name}");

			return item.ToSuccessResult(TimeSpan.FromSeconds(30));
		},
			item.DefaultFailureResult()
		);
	}
}