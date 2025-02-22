using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	public class TokenResponse : IResponse
	{
		public string Token { get; set; } = default!;
		public string RefreshToken { get; set; } = default!;
	}
}
