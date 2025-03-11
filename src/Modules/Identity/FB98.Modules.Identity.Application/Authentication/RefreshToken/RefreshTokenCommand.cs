namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	public record RefreshTokenCommand(string RefreshToken) : ICommand<ApiResult<TokenResponse>>;
}