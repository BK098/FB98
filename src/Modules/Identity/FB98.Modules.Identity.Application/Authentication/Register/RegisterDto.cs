using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Identity.Application.Authentication.Register
{
	public class RegisterDto
	{
		[EmailAddress]
		public string? Email { get; set; }
		public string? Password { get; set; }
		public int Age { get; set; }
		public string? PhoneNumber { get; set; }
		public string? Firstname { get; set; }
		public string? Lastname { get; set; }
	}
}
