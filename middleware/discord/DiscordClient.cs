using core.Configuration;
using Discord;
using Discord.WebSocket;
using Microsoft.Extensions.Logging;

namespace discord;

public class DiscordClient
{
	private readonly IEnvironmentVariableConfiguration _environmentVariableConfiguration;
	private readonly ILogger<DiscordClient> _logger;
	private DiscordSocketClient _client;

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
	
}