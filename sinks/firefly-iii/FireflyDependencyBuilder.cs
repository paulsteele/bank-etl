using Autofac;
using core.Dependencies;
using core.models;
using firefly_iii.sources;
using firefly_iii.transformers;

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