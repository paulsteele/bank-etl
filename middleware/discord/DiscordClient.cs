using core.Configuration;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace discord;

public class DiscordClient
{
	private readonly IEnvironmentVariableConfiguration _environmentVariableConfiguration;
	private readonly ILogger<DiscordClient> _logger;
	private readonly DiscordSocketClient _client;

	public DiscordClient(
		IEnvironmentVariableConfiguration environmentVariableConfiguration,
		ILogger<DiscordClient> logger
	)
	{
		_environmentVariableConfiguration = environmentVariableConfiguration;
		_logger = logger;

		_client = new DiscordSocketClient();
		_client.Log += Log;

		_client.LoginAsync(TokenType.Bot, _environmentVariableConfiguration.DiscordBotKey).Wait();
		_client.StartAsync().Wait();
	}
	
	private Task Log(LogMessage msg)
	{
		switch (msg.Severity)
		{
			case LogSeverity.Critical:
				_logger.LogCritical(msg.Message);
				break;
			case LogSeverity.Error:
				_logger.LogError(msg.Message);
				break;
			case LogSeverity.Warning:
				_logger.LogWarning(msg.Message);
				break;
			case LogSeverity.Info:
				_logger.LogInformation(msg.Message);
				break;
			case LogSeverity.Verbose:
				_logger.LogTrace(msg.Message);
				break;
			case LogSeverity.Debug:
				_logger.LogDebug(msg.Message);
				break;
		}
		return Task.CompletedTask;
	}

	private SocketTextChannel? GetChannel()
	{
		return _client.Guilds.Select(
			g =>
			{
				var guildChannel = g.Channels.FirstOrDefault(c => c.Name == _environmentVariableConfiguration.DiscordChannelName);

				return guildChannel == null ? null : g.GetTextChannel(guildChannel.Id);
			}
		).FirstOrDefault();
	}
	
	public async Task<ulong> SendMessage(string message)
	{
		var channel = GetChannel();

		if (channel == null)
		{
			_logger.LogError($"Could not find channel {_environmentVariableConfiguration.DiscordChannelName}");
			return 0;
		}

		var response = await channel.SendMessageAsync(message);

		return response.Id;
	}

	public async Task<(string Name, int ReactionCount)[]> GetReactions(ulong messageId)
	{
		var channel = GetChannel();
		if (channel == null)
		{
			_logger.LogError($"Could not find channel {_environmentVariableConfiguration.DiscordChannelName}");
			return Array.Empty<(string Name, int ReactionCount)>();
		}

		var message = await channel.GetMessageAsync(messageId);

		var reactions = message.Reactions.Select(pair => (pair.Key.Name, pair.Value.ReactionCount)).ToArray();

		return reactions;
	}
}