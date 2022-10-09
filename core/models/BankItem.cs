using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace core.models;

public class BankItem
{
	// Processing Details
	[DatabaseGenerated(DatabaseGeneratedOption.Identity)]
	[Key]
	public Guid? Id { get; set; }
	public string? State { get; set; }
	// sqs Details
	public string? RawPayload { get; set; }
	// Discord Details
	public Guid? DiscordMessage { get; set; }
	// Transaction Details
	public decimal? Amount { get; set; }
	public DateTime? Timestamp { get; set; }
	public string? Category { get; set; }
}