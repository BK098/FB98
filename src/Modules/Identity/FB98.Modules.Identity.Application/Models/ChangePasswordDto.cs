namespace FB98.Modules.Identity.Application.Models
{
	public class ChangePasswordDto
	{
		public string? CurrentPassword { get; set; }
		public string? NewPassword { get; set; }
	}
}
