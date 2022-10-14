using System.Text.Json;
using System.Text.Json.Serialization;
using core.models;

namespace ses;

public class SesTransformer : ITransformer
{
	public string SourceState => "ReceivedFromSqs";
	public Task Transform(BankItem item)
	{
		if (item.RawPayload == null)
		{
			return Task.CompletedTask;
		}

		var field = JsonSerializer.Deserialize<SesJson>(item.RawPayload);

		return Task.CompletedTask;
	}
}