using Autofac;
using core.Dependencies;
using core.models;
using ses.transformers;

namespace ses;

public class SesDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<SesBankItemTransformer>().As<SesBankItemTransformer>().SingleInstance();
	}
}