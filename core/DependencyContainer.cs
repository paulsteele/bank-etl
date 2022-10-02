using Autofac;
using core.Configuration;
using core.Database;
using Microsoft.Extensions.Logging;

namespace core;

public static class DependencyContainer
{
	private static IContainer? _instance;

	public static IContainer Instance => _instance ??= Register();

	private static IContainer Register()
	{
		var builder = new ContainerBuilder();

		builder.RegisterType<DatabaseContext>().As<DatabaseContext>().SingleInstance();
		builder.RegisterType<Db>().As<IDb>().SingleInstance();
		builder.RegisterType<EnvironmentVariableConfiguration>().As<IEnvironmentVariableConfiguration>();
		
		builder.RegisterInstance(LoggerFactory.Create(
				logBuilder =>
				{
					logBuilder.AddConsole();
				}))
			.As<ILoggerFactory>();

		builder.RegisterGeneric(typeof(Logger<>)).As(typeof(ILogger<>));

		return builder.Build();
	}
}