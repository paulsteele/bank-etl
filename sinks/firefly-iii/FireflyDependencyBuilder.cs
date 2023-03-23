using Autofac;
using core.Dependencies;
using core.models;

namespace firefly_iii;

public class FireflyDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<FireflyClient>().As<FireflyClient>().SingleInstance();
		builder.RegisterType<FireflyCategorySource>().As<FireflyCategorySource>().SingleInstance();
		builder.RegisterType<TransactionTransformer>().As<TransactionTransformer>().SingleInstance();
	}
}