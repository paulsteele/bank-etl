using System.Text.Json;
using System.Text.Json.Serialization;

namespace firefly_iii;

public class StringToDecimalConverter : JsonConverter<decimal?>
{
	public override decimal? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
	{
		var value = reader.GetString();
		var success = decimal.TryParse(value, out var decValue);

		return success ? decValue : null;
	}

	public override void Write(Utf8JsonWriter writer, decimal? value, JsonSerializerOptions options)
	{
		var writeVal = value != null ? $"{value:F2}": string.Empty;
		writer.WriteStringValue(writeVal);
	}
}