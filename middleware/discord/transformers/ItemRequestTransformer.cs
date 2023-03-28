using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace discord.transformers;

public class ItemRequestTransformer : ITransformer<BankItem>
{
	private readonly DiscordClient _client;
	private readonly ILogger<ItemRequestTransformer> _logger;
	private readonly IErrorHandler _errorHandler;

	public ItemRequestTransformer(
		DiscordClient client,
		ILogger<ItemRequestTransformer> logger,
		IErrorHandler errorHandler
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
			var categories = db.GetAllCategories();
			var pendingCategories = categories.Any(c => !c.State.Equals("Setup"));
			if (pendingCategories || !categories.Any())
			{
				_logger.LogInformation("Skipping transformation due to pending categories");
				return item.DefaultFailureResult();
			}

			var message = await _client.SendMessage($"${item.Amount:F2}\n{item.Vendor}\n{item.Timestamp}");

			if (message == 0)
			{
				_logger.LogError($"Could not send message for {item.Id}");
				return item.DefaultFailureResult();
			}

			var result = await _client.React(message, categories.Select(c => c.Emoji).ToArray());

			if (!result)
			{
				_logger.LogError($"Could not react to message {item.Id}");
				return item.DefaultFailureResult();
			}

			item.DiscordMessageId = message;

			return item.ToSuccessResult(TimeSpan.FromSeconds(30));
		},
			item.DefaultFailureResult()
		);
	}
}