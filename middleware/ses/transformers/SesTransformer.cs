using System.Text;
using System.Text.Json;
using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace ses.transformers;

public class SesBankItemTransformer : ITransformer<BankItem>
{
	private readonly ILogger<SesBankItemTransformer> _logger;
	private readonly IErrorHandler _errorHandler;

	public SesBankItemTransformer(ILogger<SesBankItemTransformer> logger, IErrorHandler errorHandler)
	{
		_logger = logger;
		_errorHandler = errorHandler;
	}
	public Task<TransformResult<BankItem>> Transform(BankItem item, IDb _)
	{
		if (item.RawPayload == null)
		{
			return Task.FromResult(item.DefaultFailureResult());
		}

		return _errorHandler.ExecuteWithErrorCatching(
			_logger, () =>
			{
				var ses = JsonSerializer.Deserialize<SesJson>(item.RawPayload);
				if (ses?.Message == null)
				{
					_logger.LogError($"Unexpected {nameof(BankItem.RawPayload)} in {item.Id}");
					return Task.FromResult(item.DefaultFailureResult());
				}

				var message = JsonSerializer.Deserialize<SesMessageJson>(ses.Message);

				if (message?.Content == null)
				{
					_logger.LogError($"Unexpected {nameof(BankItem.RawPayload)} in {item.Id}");
					return Task.FromResult(item.DefaultFailureResult());
				}

				item.RawEmail = Encoding.UTF8.GetString(Convert.FromBase64String(message.Content));
				_logger.LogInformation($"Successfully parsed {item.Id}");
				return Task.FromResult(item.ToSuccessResult(TimeSpan.FromSeconds(30)));
			},
			item.DefaultFailureResult()
			);
	}
}