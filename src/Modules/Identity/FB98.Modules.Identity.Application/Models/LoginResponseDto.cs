namespace FB98.Modules.Identity.Application.Models
{
	public class LoginResponseDto
	{
		public string Token { get; set; } = default!;
		public DateTime Expiration { get; set; }
	}

}

