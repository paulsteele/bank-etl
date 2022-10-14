using System.Text.Json.Serialization;

namespace ses;

public class SesMessageMailJson
{
	
	[JsonPropertyName("content")]
	string Content { get; set; }
	
}