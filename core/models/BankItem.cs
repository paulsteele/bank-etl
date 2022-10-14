using System.ComponentModel.DataAnnotations;

namespace core.models;

public class BankItem
{
	// Processing Details
	[Key]
	[Required]
	public Guid? Id { get; set; }
	public string? State { get; set; }
	// sqs Details
	public string? RawPayload { get; set; }
	public string? RawEmail { get; set; }
	// Discord Details
	public Guid? DiscordMessageId { get; set; }
	// Transaction Details
	public decimal? Amount { get; set; }
	public DateTime? Timestamp { get; set; }
	public string? Category { get; set; }
}