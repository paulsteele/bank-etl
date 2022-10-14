using System.Text.Json.Serialization;

namespace ses;

public class SesJson
{
	
	[JsonPropertyName("Message")]
	public string? Message { get; set; }
}