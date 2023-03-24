using Autofac;
using core.Dependencies;
using core.models;
using discord.transformers;

namespace discord;

public class DiscordDependencyBuilder : IDependencyContainerBuilder
{
	public void RegisterDependencies(ContainerBuilder builder)
	{
		builder.RegisterType<DiscordClient>().As<DiscordClient>().SingleInstance();
		builder.RegisterType<CategoryEmojiRequestTransformer>().As<CategoryEmojiRequestTransformer>().SingleInstance();
		builder.RegisterType<CategoryEmojiResponseTransformer>().As<CategoryEmojiResponseTransformer>().SingleInstance();
		builder.RegisterType<ItemRequestTransformer>().As<ItemRequestTransformer>().SingleInstance();
		builder.RegisterType<ItemResponseTransformer>().As<ItemResponseTransformer>().SingleInstance();
		builder.RegisterType<ItemRemainingTransformer>().As<ItemRemainingTransformer>().SingleInstance();
	}
}