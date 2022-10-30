using core.Configuration;
using core.models;
using Microsoft.EntityFrameworkCore;
using Microsoft.EntityFrameworkCore.Design;

namespace core.Database;

public class DatabaseContext : DbContext
{
	private readonly IEnvironmentVariableConfiguration _configuration;
	
	public DbSet<BankItem> Items { get; set; }
	public DbSet<Category> Categories { get; set; }

	public DatabaseContext(IEnvironmentVariableConfiguration configuration)
	{
		_configuration = configuration;
	}

	protected override void OnConfiguring(DbContextOptionsBuilder optionsBuilder)
	{
		var connectionString = $"Server={_configuration.DatabaseUrl};Port={_configuration.DatabasePort};Database={_configuration.DatabaseName};Uid={_configuration.DatabaseUser};Pwd={_configuration.DatabasePassword};";
		var serverVersion = ServerVersion.AutoDetect(connectionString);
		optionsBuilder.UseMySql(connectionString, serverVersion).EnableDetailedErrors();
		optionsBuilder.EnableDetailedErrors();
		optionsBuilder.EnableSensitiveDataLogging();
	}
	
	public class MigrationContextFactory : IDesignTimeDbContextFactory<DatabaseContext>
	{
		public DatabaseContext CreateDbContext(string[] args)
		{
			return new(new EnvironmentVariableConfiguration());
		}
	}
}