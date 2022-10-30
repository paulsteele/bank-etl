using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class BudgetSpent
{
	[JsonPropertyName("sum")]
	public string Sum { get; set; }
	
	[JsonPropertyName("currency_id")]
	public string CurrencyId { get; set; }
	
	[JsonPropertyName("currency_code")]
	public string CurrencyCode { get; set; }
	
	[JsonPropertyName("currency_symbol")]
	public string CurrencySymbol { get; set; }
	
	[JsonPropertyName("currency_decimal_places")]
	public int CurrencyDecimalPlaces { get; set; }
}
