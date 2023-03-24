using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace firefly_iii.sources;

public class FireflyCategorySource : ISource<Category>
{
	private readonly FireflyClient _client;
	private readonly ILogger<FireflyCategorySource> _logger;
	private readonly ErrorHandler _errorHandler;

	public FireflyCategorySource(
		FireflyClient client, 
		ILogger<FireflyCategorySource> logger,
		ErrorHandler errorHandler
	)
	{
		_client = client;
		_logger = logger;
		_errorHandler = errorHandler;
	}

	public Task<TimeSpan> Poll(IDb database, string successState)
	{
		return _errorHandler.ExecuteWithErrorCatching(_logger, async () =>
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
			return TimeSpan.FromDays(1);
		}, 
			TimeSpan.FromSeconds(30));
	}
}