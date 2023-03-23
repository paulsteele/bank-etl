using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace firefly_iii;

public class TransactionTransformer : ITransformer<BankItem>
{
	private readonly FireflyClient _client;
	private readonly ILogger<TransactionTransformer> _logger;

	public TransactionTransformer(
		FireflyClient client,
		ILogger<TransactionTransformer> logger
	)
	{
		_client = client;
		_logger = logger;
	}

	public string SourceState => "ReceivedEmoji";
	private string DestinationState => "SentToFirefly";
	public async Task Transform(BankItem item, IDb db)
	{
		db.LoadForeignKeys(item);

		if (item.Category == null)
		{
			_logger.LogError($"{item.Id} has no category");
			return;
		}
		
		var success = await _client.SendTransaction(item);

		if (!success)
		{
			_logger.LogError($"Could not send {item.Id} to firefly");
			return;
		}

		var budget = (await _client.GetBudgets()).FirstOrDefault(b => b.Id == item.Category.FireflyId);

		if (budget == null || budget.Attributes == null || budget.Attributes.Spent.Length != 1)
		{
			_logger.LogError($"Budget for {item.Category.FireflyId} could not be found");
			return;
		}

		var budgeted = budget.Attributes.BudgetAmount ?? 0m;

		item.AmountInCategoryAfter = budgeted + budget.Attributes.Spent.FirstOrDefault()!.Sum;
		item.State = DestinationState;
	}
}