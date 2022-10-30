using Autofac;
using core.Dependencies;
using core.models;

namespace ses;

public class SesDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<SesBankItemTransformer>().As<IBankItemTransformer>().SingleInstance();
	}
}