using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.Authentication.Logout
{
	public record LogoutCommand(string UserId) : ICommand<ApiResponse<object>>;
}
