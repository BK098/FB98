using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Identity.Application.Authentication.Login
{
	public class LoginDto
	{
		[EmailAddress]
		public string? Email { get; set; }
		public string? Password { get; set; }
	}
}
