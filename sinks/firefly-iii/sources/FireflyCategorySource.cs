using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace firefly_iii.sources;

public class FireflyCategorySource : ISource<Category>
{
	private readonly FireflyClient _client;
	private readonly ILogger<FireflyCategorySource> _logger;

	public FireflyCategorySource(
		FireflyClient client, 
		ILogger<FireflyCategorySource> logger
	)
	{
		_client = client;
		_logger = logger;
	}

	public async Task Poll(IDb database, string successState)
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
				FireflyOrder = b.Attributes.Order,
				State = successState
			}
		);
		
		foreach (var category in categories)
		{
			database.AddCategory(category);
			_logger.LogInformation($"Added Category {category.Name}");
		}
		database.SaveChanges();
	}
}