using IContainer = Autofac.IContainer;

namespace core;

public static class DependencyContainer
{
	public static IContainer Instance { get; set; }
}