using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Identity.Application.Models
{
	public class ForgotPasswordDto
	{
		[EmailAddress]
		public string? Email {  get; set; }
	}
}
