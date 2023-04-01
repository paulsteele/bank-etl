using System.Text.RegularExpressions;
using core;
using core.Db;
using core.models;
using HtmlAgilityPack;
using Microsoft.Extensions.Logging;

namespace chase.transformers;

public partial class ChaseBankItemTransformer : ITransformer<BankItem>
{
	private readonly ILogger<ChaseBankItemTransformer> _logger;
	private readonly IErrorHandler _errorHandler;

	public ChaseBankItemTransformer(ILogger<ChaseBankItemTransformer> logger, IErrorHandler errorHandler)
	{
		_logger = logger;
		_errorHandler = errorHandler;
	}
	
	public Task<TransformResult<BankItem>> Transform(BankItem item, IDb _)
	{
		return _errorHandler.ExecuteWithErrorCatching(
			_logger, () =>
			{
				var snippet = new HtmlDocument();
				
				snippet.LoadHtml(item.RawEmail);

				var eligibleRows = snippet.DocumentNode.Descendants()
					.Where(node =>
						node.Name == "tr" &&
						node.ChildNodes.Count == 5) // Text nodes surround / separate the 2 actual children we care about
					.Select(row => row.ChildNodes.Where(a => a.Name == "td"));
				

				/*
				item.Amount = decimal.Parse(amount);
				item.Vendor = location;
				item.Timestamp = DateTimeOffset.Now;
				*/
				_logger.LogInformation($"Successfully parsed {item.Id} - {item.Amount} - {item.Vendor}");
				return Task.FromResult(item.ToSuccessResult(TimeSpan.FromSeconds(30)));
			},
			item.DefaultFailureResult()
		);
	}
}