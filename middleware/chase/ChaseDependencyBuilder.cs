using Autofac;
using core.Dependencies;
using core.models;

namespace chase;

public class ChaseDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<ChaseTransformer>().As<ITransformer>().SingleInstance();
	}
}