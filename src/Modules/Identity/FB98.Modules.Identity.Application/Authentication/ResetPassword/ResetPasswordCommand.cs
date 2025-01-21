using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.Authentication.ResetPassword
{
	public record ResetPasswordCommand(ResetPasswordDto Model) : ICommand<ApiResponse<object>>;
}
