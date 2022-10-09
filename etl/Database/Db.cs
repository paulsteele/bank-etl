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
		}

		public BankItem UpsertBankItem(BankItem item)
		{
			return item.Id == Guid.Empty ? 
				_databaseContext.Items.Add(item).Entity : 
				_databaseContext.Items.Update(item).Entity;
		}

		public void SaveChanges()
		{
			_databaseContext.SaveChanges();
		}
	}
}
