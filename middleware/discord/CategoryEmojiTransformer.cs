using core.models;

namespace discord;

public class CategoryEmojiTransformer : ICategoryTransformer
{
	private readonly DiscordClient _client;

	public CategoryEmojiTransformer(DiscordClient client)
	{
		_client = client;
	}
	public string SourceState => "ReceivedFromFirefly";
	public Task Transform(Category item)
	{
		return Task.CompletedTask;
	}
}