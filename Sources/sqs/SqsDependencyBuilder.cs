using Autofac;
using core.Dependencies;
using core.models;
using sqs.sources;

namespace sqs;

public class SqsDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<EmailQueue>().As<EmailQueue>().SingleInstance();
	}
}