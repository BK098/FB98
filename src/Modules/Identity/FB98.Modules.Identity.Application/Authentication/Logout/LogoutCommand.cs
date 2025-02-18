namespace FB98.Modules.Identity.Application.Authentication.Logout
{
	public record LogoutCommand(string UserId) : ICommand<ApiResult<object>>;
}
