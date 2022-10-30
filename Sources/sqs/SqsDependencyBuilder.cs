using Autofac;
using core.Dependencies;
using core.models;

namespace sqs;

public class SqsDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<EmailQueue>().As<IBankItemSource>().SingleInstance();
	}
}