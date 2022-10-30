using Autofac;
using core.Dependencies;
using core.models;

namespace discord;

public class DiscordDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<DiscordClient>().As<DiscordClient>().SingleInstance();
		builder.RegisterType<CategoryEmojiTransformer>().As<ICategoryTransformer>().SingleInstance();
	}
}