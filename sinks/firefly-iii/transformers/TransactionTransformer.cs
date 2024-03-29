using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace firefly_iii.transformers;

public class TransactionTransformer : ITransformer<BankItem>
{
	private readonly FireflyClient _client;
	private readonly ILogger<TransactionTransformer> _logger;
	private readonly IErrorHandler _errorHandler;

	public TransactionTransformer(
		FireflyClient client,
		ILogger<TransactionTransformer> logger,
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
			db.LoadForeignKeys(item);

			if (item.Category == null)
			{
				_logger.LogError($"{item.Id} has no category");
				return item.DefaultFailureResult();
			}

			var success = await _client.SendTransaction(item);

			if (!success)
			{
				_logger.LogError($"Could not send {item.Id} to firefly");
				return item.DefaultFailureResult();
			}

			var budget = (await _client.GetBudgets()).FirstOrDefault(b => b.Id == item.Category.FireflyId);

			if (budget == null || budget.Attributes == null || budget.Attributes.Spent.Length != 1)
			{
				_logger.LogError($"Budget for {item.Category.FireflyId} could not be found");
				return item.DefaultFailureResult();
			}

			var budgeted = budget.Attributes.BudgetAmount ?? 0m;

			item.AmountInCategoryAfter = budgeted + budget.Attributes.Spent.FirstOrDefault()!.Sum;

			return item.ToSuccessResult(TimeSpan.FromSeconds(30));
		},
			item.DefaultFailureResult()
		);
	}
}