using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class BudgetSpent
{
	[JsonPropertyName("sum")]
	[JsonConverter(typeof(StringToDecimalConverter))]
	public decimal? Sum { get; set; }
	
	[JsonPropertyName("currency_id")]
	public int CurrencyId { get; set; }
	
	[JsonPropertyName("currency_code")]
	public string CurrencyCode { get; set; }
	
	[JsonPropertyName("currency_symbol")]
	public string CurrencySymbol { get; set; }
	
	[JsonPropertyName("currency_decimal_places")]
	public int CurrencyDecimalPlaces { get; set; }
}
