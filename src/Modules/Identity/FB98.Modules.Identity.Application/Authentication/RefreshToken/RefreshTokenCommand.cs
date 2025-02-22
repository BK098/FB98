namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	public record RefreshTokenCommand(RefreshTokenDto Model) : ICommand<ApiResult<TokenResponse>>;
}
