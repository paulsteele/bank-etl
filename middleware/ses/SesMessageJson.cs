using System.Text.Json.Serialization;

namespace ses;

public class SesMessageJson
{
	[JsonPropertyName("notificationType")]
	string NotificationType { get; set; }
	
	[JsonPropertyName("mail")]
	SesMessageMailJson Mail { get; set; }
	
}