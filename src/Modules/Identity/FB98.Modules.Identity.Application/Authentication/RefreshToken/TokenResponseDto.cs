namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	public class TokenResponseDto
	{
		public string Token { get; set; } = default!;
		public string RefreshToken { get; set; } = default!;
	}
}
