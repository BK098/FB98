namespace FB98.Modules.Identity.Application.Authentication.Register
{
	public record RegisterCommand(RegisterDto Model) : ICommand<ApiResult<object>>;
}