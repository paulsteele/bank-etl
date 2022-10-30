using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class Budget
{
	[JsonPropertyName("type")]
	public string Type { get; set; }
	
	[JsonPropertyName("id")]
	public string Id { get; set; }
	
	[JsonPropertyName("attributes")]
	public BudgetAttributes? Attributes { get; set; }
	
}