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
	public ulong? DiscordMessageId { get; set; }
	// Transaction Details
	public decimal? Amount { get; set; }
	public decimal? AmountInCategoryAfter { get; set; }
	public string? Vendor { get; set; }
	public DateTimeOffset? Timestamp { get; set; }
	public Category? Category { get; set; }
}