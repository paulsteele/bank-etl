using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class Transaction
{
	[JsonPropertyName("type")]
  public string Type { get; set; }
	
	[JsonPropertyName("date")]
  public DateTimeOffset Date { get; set; }
  
	[JsonPropertyName("amount")]
  public string Amount { get; set; }
	
	[JsonPropertyName("description")]
  public string Description { get; set; }
	
  [JsonPropertyName("budget_id")]
  public string BudgetId  { get; set; }
  
  [JsonPropertyName("source_id")]
  public string SourceId  { get; set; }
}