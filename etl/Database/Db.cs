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

			return _databaseContext.Add(item).Entity;
		}

		public void SaveChanges()
		{
			_databaseContext.SaveChanges();
		}
	}
}
