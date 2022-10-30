using System.ComponentModel.DataAnnotations;

namespace core.models;

public class Category
{
	[Key]
	[Required]
	public Guid? Id { get; set; }
	public string Name { get; set; }
	public string State { get; set; }
	public string FireflyId { get; set; }
	public char? Emoji { get; set; }
}