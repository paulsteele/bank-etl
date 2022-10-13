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
	public string SourceState => "ReceivedFromSqs";
	
	public Task Transform(IEnumerable<BankItem> items)
	{
		foreach (var item in items)
		{
			var payload = item.RawPayload;
		}
		
		return Task.CompletedTask;
	}
}