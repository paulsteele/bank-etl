using System.Text;
using System.Text.Json;
using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace ses;

public class SesBankItemTransformer : ITransformer<BankItem>
{
	private readonly ILogger<SesBankItemTransformer> _logger;
	private readonly ErrorHandler _errorHandler;
	public string SourceState => "ReceivedFromSqs";
	private string DestinationState => "ParsedFromSes";

	public SesBankItemTransformer(ILogger<SesBankItemTransformer> logger, ErrorHandler errorHandler)
	{
		_logger = logger;
		_errorHandler = errorHandler;
	}
	public Task Transform(BankItem item, IDb _)
	{
		if (item.RawPayload == null)
		{
			return Task.CompletedTask;
		}

		_errorHandler.ExecuteWithErrorCatching(
			_logger, () =>
			{
				var ses = JsonSerializer.Deserialize<SesJson>(item.RawPayload);
				if (ses?.Message == null)
				{
					_logger.LogError($"Unexpected {nameof(BankItem.RawPayload)} in {item.Id}");
					return;
				}
				var message = JsonSerializer.Deserialize<SesMessageJson>(ses.Message);

				if (message?.Content == null)
				{
					_logger.LogError($"Unexpected {nameof(BankItem.RawPayload)} in {item.Id}");
					return;
				}
				
				item.RawEmail = Encoding.UTF8.GetString(Convert.FromBase64String(message.Content));
				item.State = DestinationState;
				_logger.LogInformation($"Successfully parsed {item.Id}");
			}
		);

		return Task.CompletedTask;
	}
}