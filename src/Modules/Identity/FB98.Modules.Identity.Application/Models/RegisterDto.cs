namespace FB98.Modules.Identity.Application.Models
{
	public class RegisterDto
	{
		public string Email { get; set; } = default!;
		public string Password { get; set; } = default!;
		public int Age { get; set; }
		public string PhoneNumber { get; set; }
		public string Firstname { get; set; }
		public string Lastname { get; set; }
	}
}
