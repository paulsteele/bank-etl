using System.Text.RegularExpressions;
using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace chase;

public class ChaseBankItemTransformer : ITransformer<BankItem>
{
	private readonly ILogger<ChaseBankItemTransformer> _logger;
	private readonly ErrorHandler _errorHandler;

	public ChaseBankItemTransformer(ILogger<ChaseBankItemTransformer> logger, ErrorHandler errorHandler)
	{
		_logger = logger;
		_errorHandler = errorHandler;
	}
	public string SourceState => "ParsedFromSes";
	private string DestinationState => "ParsedFromChase";
	
	public Task Transform(BankItem item, IDb _)
	{
		_errorHandler.ExecuteWithErrorCatching(
			_logger, () =>
			{
				var match = Regex.Match(item.RawEmail, "\\$((\\d*,?)*\\.+\\d*)\\s*(with)*(to)*\\s*(.*)</td>");
				var amount = match.Groups[1].Value;
				var location = match.Groups[5].Value;

				if (string.IsNullOrWhiteSpace(amount) || string.IsNullOrWhiteSpace(location))
				{
					_logger.LogError($"Could not parse Chase Email {item.Id}");
					return;
				}

				item.Amount = decimal.Parse(amount);
				item.Vendor = location;
				item.State = DestinationState;
				item.Timestamp = DateTimeOffset.Now;
				_logger.LogInformation($"Successfully parsed {item.Id} - {item.Amount} - {item.Vendor}");
			}
		);
		return Task.CompletedTask;
	}
}