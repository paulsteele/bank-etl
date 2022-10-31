using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class TransactionRequest
{
	[JsonPropertyName("transactions")]
  public Transaction[] Transactions { get; set; }
}