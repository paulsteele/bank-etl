using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace firefly_iii;

public class FireflyCategorySource : ICategorySource
{
	private readonly FireflyClient _client;
	private readonly ILogger<FireflyCategorySource> _logger;
	private const string ReceivedFromFirefly = nameof(ReceivedFromFirefly);

	public FireflyCategorySource(
		FireflyClient client, 
		ILogger<FireflyCategorySource> logger
	)
	{
		_client = client;
		_logger = logger;
	}

	public async Task Poll(IDb database)
	{
		var budgets = await _client.GetBudgets();

		var existingCategoriesIds = database.GetAllCategories().Select(category => category.FireflyId).ToArray();

		var newBudgets = budgets.Where(b => !existingCategoriesIds.Contains(b.Id));

		var validBudgets = newBudgets.Where(
			b =>
			{
				if (b.Attributes == null)
				{
					_logger.LogError($"{b} does not contain attributes");

					return false;
				}

				if (string.IsNullOrWhiteSpace(b.Attributes.Name))
				{
					_logger.LogError($"{b} does not contain a name");
					return false;
				}

				return true;

			}
		);

		var categories = validBudgets.Select(
			b => new Category
			{
				Name = b.Attributes!.Name,
				FireflyId = b.Id,
				State = ReceivedFromFirefly
			}
		);
		
		foreach (var category in categories)
		{
			database.AddCategory(category);
			_logger.LogInformation($"Added Category {category.Name}");
		}
	}
}