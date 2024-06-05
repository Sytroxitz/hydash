using System.ComponentModel.DataAnnotations;

public class RegisterModel
{
	[Required]
	[MinLength(4)]
	public string Username { get; set; }

	[Required]
	[EmailAddress]
	public string Email { get; set; }

	[Required]
	[DataType(DataType.Password)]
	public string Password { get; set; }
}