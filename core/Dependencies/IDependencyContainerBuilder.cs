using Autofac;

namespace core.Dependencies;

public interface IDependencyContainerBuilder
{
	void RegisterDependencies(ContainerBuilder builder);
}