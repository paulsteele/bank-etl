using Autofac;
using core.Configuration;
using core.Database;
using core.Db;
using core.Dependencies;
using Microsoft.Extensions.Logging;
using sqs;

namespace core;

public class EtlDependencyContainerBuilder : IDependencyContainerBuilder
{
	public static void RegisterContainer()
	{
		var builder = new ContainerBuilder();
		
		new EtlDependencyContainerBuilder().RegisterDependencies(builder);
		new SqsDependencyBuilder().RegisterDependencies(builder);

		DependencyContainer.Instance =  builder.Build();
	}

	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<DatabaseContext>().As<DatabaseContext>();
		builder.RegisterType<Database.Db>().As<IDb>().SingleInstance();
		builder.RegisterType<EnvironmentVariableConfiguration>().As<IEnvironmentVariableConfiguration>();
		
		builder.RegisterInstance(LoggerFactory.Create(
				logBuilder =>
				{
					logBuilder.AddConsole();
				}))
			.As<ILoggerFactory>();

		builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));
	}
}