using Autofac;
using core.Db;
using core.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace core.Database {
	public class Db : IDb {
		private readonly ILogger<Db> _logger;
		private readonly ILifetimeScope _lifetimeScope;
		private ILifetimeScope _databaseScope;
		private DatabaseContext? _databaseContext;

		public Db(ILogger<Db> logger, ILifetimeScope lifetimeScope)
		{
			_logger = logger;
			_lifetimeScope = lifetimeScope;
		}

		public void Init() {
			_logger.LogInformation("Initializing Database");
			Context.Database.Migrate();

			_databaseContext = null;
			_databaseScope.Dispose();
		}

		public BankItem? AddItem(BankItem item)
		{
			if (item.Id != null)
			{
				_logger.LogError($"{nameof(AddItem)} can only be called on a new {nameof(BankItem)}");
			}

			return Context.Items.Add(item).Entity;
		}

		private DatabaseContext Context
		{
			get
			{
				// ReSharper disable once InvertIf
				if (_databaseContext == null)
				{
					_databaseScope = _lifetimeScope.BeginLifetimeScope();
					_databaseContext = _databaseScope.Resolve<DatabaseContext>();
				}

				return _databaseContext;
			}
		}

		public void SaveChanges()
		{
			Context.SaveChanges();
			_databaseContext = null;
			_databaseScope.Dispose();
		}
	}
}
