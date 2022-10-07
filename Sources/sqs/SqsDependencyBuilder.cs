using Autofac;
using core.Dependencies;

namespace sqs;

public class SqsDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<EmailQueue>().SingleInstance();
	}
}