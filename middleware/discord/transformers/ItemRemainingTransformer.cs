using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord.transformers;

public class ItemRemainingTransformer : ITransformer<BankItem>
{
	private readonly DiscordClient _client;
	private readonly ILogger<ItemRemainingTransformer> _logger;
	private readonly ErrorHandler _errorHandler;

	public ItemRemainingTransformer(
		DiscordClient client,
		ILogger<ItemRemainingTransformer> logger,
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
			db.LoadForeignKeys(item);
			if (!item.DiscordMessageId.HasValue)
			{
				_logger.LogError($"{item.Id} does not have a {nameof(BankItem.DiscordMessageId)}");
				return item.DefaultFailureResult();
			}

			if (item.Category == null)
			{
				_logger.LogError($"{item.Id} does not have a {nameof(BankItem.Category)}");
				return item.DefaultFailureResult();
			}

			await _client.SendMessage($"Balance for {item.Category.Name}:\n${item.AmountInCategoryAfter:F2}");

			await _client.React(item.DiscordMessageId.Value, new[] { "✅" });
			return item.ToSuccessResult(TimeSpan.FromSeconds(30));
		});
	}
}