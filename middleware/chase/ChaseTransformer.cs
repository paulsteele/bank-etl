using System.Text.RegularExpressions;
using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace chase;

public class ChaseBankItemTransformer : IBankItemTransformer
{
	private readonly ILogger<ChaseBankItemTransformer> _logger;

	public ChaseBankItemTransformer(ILogger<ChaseBankItemTransformer> logger)
	{
		_logger = logger;
	}
	public string SourceState => "ParsedFromSes";
	private string DestinationState => "ParsedFromChase";
	
	public Task Transform(BankItem item, IDb _)
	{
		ErrorCatching.ExecuteWithErrorCatching(
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
				_logger.LogInformation($"Successfully parsed {item.Id} - {item.Amount} - {item.Vendor}");
			}
		);
		return Task.CompletedTask;
	}
}