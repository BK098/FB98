using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Identity.Application.Authentication.ForgotPassword
{
	public class ForgotPasswordDto
	{
		[EmailAddress]
		public string? Email { get; set; }
	}
}
