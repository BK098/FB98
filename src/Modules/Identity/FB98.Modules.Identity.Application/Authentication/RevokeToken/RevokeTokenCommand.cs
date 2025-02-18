namespace FB98.Modules.Identity.Application.Authentication.RevokeToken
{
	public record RevokeTokenCommand(Guid UserId) : ICommand<ApiResult<object>>;
}
