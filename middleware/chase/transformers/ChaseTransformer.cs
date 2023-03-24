using System.Text.RegularExpressions;
using core;
using core.Db;
using core.models;
using Microsoft.Extensions.Logging;

namespace chase.transformers;

public partial class ChaseBankItemTransformer : ITransformer<BankItem>
{
	private readonly ILogger<ChaseBankItemTransformer> _logger;
	private readonly ErrorHandler _errorHandler;

	public ChaseBankItemTransformer(ILogger<ChaseBankItemTransformer> logger, ErrorHandler errorHandler)
	{
		_logger = logger;
		_errorHandler = errorHandler;
	}
	
	[GeneratedRegex("\\$((\\d*,?)*\\.+\\d*)\\s*(with)*(to)*\\s*(.*)</td>")]
	private static partial Regex ChaseEmailRegex();
	
	public Task<TransformResult<BankItem>> Transform(BankItem item, IDb _)
	{
		return _errorHandler.ExecuteWithErrorCatching(
			_logger, () =>
			{
				var match = ChaseEmailRegex().Match(item.RawEmail);
				var amount = match.Groups[1].Value;
				var location = match.Groups[5].Value;

				if (string.IsNullOrWhiteSpace(amount) || string.IsNullOrWhiteSpace(location))
				{
					_logger.LogError($"Could not parse Chase Email {item.Id}");
					return Task.FromResult(item.DefaultFailureResult());
				}

				item.Amount = decimal.Parse(amount);
				item.Vendor = location;
				item.Timestamp = DateTimeOffset.Now;
				_logger.LogInformation($"Successfully parsed {item.Id} - {item.Amount} - {item.Vendor}");
				return Task.FromResult(item.ToSuccessResult(TimeSpan.FromSeconds(30)));
			},
			item.DefaultFailureResult()
		);
	}
}