using Autofac;
using core.Dependencies;
using core.models;

namespace chase;

public class ChaseDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<ChaseBankItemTransformer>().As<ChaseBankItemTransformer>().SingleInstance();
	}
}