namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	public class RefreshTokenDto
	{
		public string Token { get; set; } = default!;
		public string RefreshToken { get; set; } = default!;
	}

}