using System.Text.Json.Serialization;

namespace ses;

public class SesJson
{
	[JsonPropertyName("Type")]
	string Type { get; set; }
	
	[JsonPropertyName("MessagedId")]
	string MessageId { get; set; }
	
	[JsonPropertyName("TopicArn")]
	string TopicArn { get; set; }
	
	[JsonPropertyName("Subject")]
	string Subject { get; set; }
	
	[JsonPropertyName("Message")]
	SesMessageJson Message { get; set; }
	
	[JsonPropertyName("Timestamp")]
	string Timestamp { get; set; }
	
	[JsonPropertyName("SignatureVersion")]
	string SignatureVersion { get; set; }
	
	[JsonPropertyName("Signature")]
	string Signature { get; set; }
	
	[JsonPropertyName("SigningCertUrl")]
	string SigningCertUrl { get; set; }
	
	[JsonPropertyName("UnsubscribeUrl")]
	string UnsubscribeUrl { get; set; }
	
}