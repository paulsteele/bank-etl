using Autofac;
using core.Dependencies;
using core.models;

namespace discord;

public class DiscordDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<DiscordClient>().As<DiscordClient>().SingleInstance();
		builder.RegisterType<CategoryEmojiRequestTransformer>().As<ICategoryTransformer>().SingleInstance();
		builder.RegisterType<CategoryEmojiResponseTransformer>().As<ICategoryTransformer>().SingleInstance();
		builder.RegisterType<ItemRequestTransformer>().As<IBankItemTransformer>().SingleInstance();
		builder.RegisterType<ItemResponseTransformer>().As<IBankItemTransformer>().SingleInstance();
	}
}