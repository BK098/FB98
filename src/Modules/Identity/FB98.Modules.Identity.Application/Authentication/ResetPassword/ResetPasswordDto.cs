using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Identity.Application.Authentication.ResetPassword
{
	public class ResetPasswordDto
	{
		[EmailAddress]
		public string? Email { get; set; }
		public string? Token { get; set; }
		public string? Password { get; set; }
	}
}