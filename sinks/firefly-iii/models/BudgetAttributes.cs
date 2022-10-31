using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class BudgetAttributes
{
	[JsonPropertyName("created_at")]
	public DateTimeOffset CreatedAt { get; set; }
	
	[JsonPropertyName("updated_at")]
	public DateTimeOffset UpdatedAt { get; set; }
	
	[JsonPropertyName("name")]
	public string Name { get; set; }
	
	[JsonPropertyName("active")]
	public bool Active { get; set; }
	
	[JsonPropertyName("notes")]
	public string Notes { get; set; }
	
	[JsonPropertyName("order")]
	public int Order { get; set; }
	
	[JsonPropertyName("auto_budget_type")]
	public string BudgetType { get; set; }
	
	[JsonPropertyName("auto_budget_currency_id")]
	public string BugetCurrencyId { get; set; }
	
	[JsonPropertyName("auto_budget_currency_code")]
	public string BugetCurrencyCode { get; set; }
	
	[JsonPropertyName("auto_budget_amount")]
	[JsonConverter(typeof(StringToDecimalConverter))]
	public decimal? BudgetAmount { get; set; }
	
	[JsonPropertyName("auto_budget_period")]
	public string BudgetPeriod { get; set; }
	
	[JsonPropertyName("spent")]
	public BudgetSpent[] Spent { get; set; }
}
