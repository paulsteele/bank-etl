using System.Text.Json.Serialization;

namespace ses;

public class SesMessageJson
{
	
	[JsonPropertyName("content")]
	public string? Content { get; set; }
}