using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Identity.Application.Authentication.Login
{
	public class LoginResponse : IResponse
	{
		public string Token { get; set; } = default!;
		public string RefreshToken { get; set; } = default!;
		public DateTime Expiration { get; set; }
	}
}