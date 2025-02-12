namespace FB98.Modules.Identity.Application.ProfileManagement.ChangePassword
{
	public record ChangePasswordCommand(string UserId, ChangePasswordDto Model) : ICommand<ApiResponse<object>>;
}
