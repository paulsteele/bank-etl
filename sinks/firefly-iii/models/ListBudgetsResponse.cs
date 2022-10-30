using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class ListBudgetsResponse
{
	[JsonPropertyName("data")]
	public Budget[] Data { get; set; }
	
	[JsonPropertyName("meta")]
	public Meta Meta { get; set; }
}