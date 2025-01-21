using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.ProfileManagement.ChangePassword
{
	public record ChangePasswordCommand(string UserId, ChangePasswordDto Model) : ICommand<ApiResponse<object>>;
}
