using System.Text.Json.Serialization;

namespace firefly_iii.models;

public class Meta
{
	[JsonPropertyName("pagination")]
	public Pagination Pagination { get; set; }
}