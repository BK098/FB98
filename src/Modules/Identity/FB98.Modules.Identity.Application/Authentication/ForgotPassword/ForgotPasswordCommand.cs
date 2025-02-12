namespace FB98.Modules.Identity.Application.Authentication.ForgotPassword
{
	public record ForgotPasswordCommand(ForgotPasswordDto Model) : ICommand<ApiResponse<object>>;
}