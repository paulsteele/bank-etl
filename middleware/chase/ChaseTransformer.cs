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
	
	public Task Transform(BankItem item)
	{
		return Task.CompletedTask;
	}
}