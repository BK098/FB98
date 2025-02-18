namespace FB98.Modules.Identity.Application.Authentication.ResetPassword
{
	public record ResetPasswordCommand(ResetPasswordDto Model) : ICommand<ApiResult<object>>;
}
