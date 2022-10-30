using core.Db;
using core.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace core.Database {
	public class Db : IDb {
		private readonly ILogger<Db> _logger;
		private readonly DatabaseContext _databaseContext;

		public Db(ILogger<Db> logger, DatabaseContext databaseContext)
		{
			_logger = logger;
			_databaseContext = databaseContext;
		}

		public void Init() {
			_logger.LogInformation("Initializing Database");
			_databaseContext.Database.Migrate();
			_logger.LogInformation("Database Initialized");
		}

		public BankItem AddItem(BankItem item)
		{
			if (item.Id != null)
			{
				_logger.LogError($"{nameof(AddItem)} can only be called on a new {nameof(BankItem)}");
			}
			
			item.Id = Guid.NewGuid();

			return _databaseContext.Add(item).Entity;
		}

		public BankItem[] GetItemsFromState(string state)
		{
			return _databaseContext.Items.Where(item => state.Equals(item.State)).ToArray();
		}

		public Category? AddCategory(Category category)
		{
			if (category.Id != null)
			{
				_logger.LogError($"{nameof(AddCategory)} can only be called on a new {nameof(Category)}");
			}

			category.Id = Guid.NewGuid();

			return _databaseContext.Add(category).Entity;
		}

		public Category[] GetCategoriesFromState(string state)
		{
			return _databaseContext.Categories.Where(category => state.Equals(category.State)).ToArray();
		}
		
		public Category[] GetAllCategories()
		{
			return _databaseContext.Categories.ToArray();
		}

		public void SaveChanges()
		{
			_databaseContext.SaveChanges();
		}
	}
}
