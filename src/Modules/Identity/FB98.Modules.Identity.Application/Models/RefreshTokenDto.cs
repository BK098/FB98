namespace FB98.Modules.Identity.Application.Models
{
	public class RefreshTokenDto
	{
		public string Token { get; set; } = default!;
		public string RefreshToken { get; set; } = default!;
	}

}