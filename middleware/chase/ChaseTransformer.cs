using System.Text.RegularExpressions;
using core;
using core.models;
using Microsoft.Extensions.Logging;

namespace chase;

public class ChaseTransformer : ITransformer
{
	private readonly ILogger<ChaseTransformer> _logger;

	public ChaseTransformer(ILogger<ChaseTransformer> logger)
	{
		_logger = logger;
	}
	public string SourceState => "ParsedFromSes";
	private string DestinationState => "ParsedFromChase";
	
	public Task Transform(BankItem item)
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
			}
		);
		return Task.CompletedTask;
	}
}