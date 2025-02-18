namespace FB98.Modules.Identity.Application.Authentication.Login
{
	public record LoginCommand(LoginDto Model) : ICommand<ApiResult<LoginResponseDto>>;
}