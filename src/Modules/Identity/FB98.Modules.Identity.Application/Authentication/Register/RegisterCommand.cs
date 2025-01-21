using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.Authentication.Register
{
	public record RegisterCommand(RegisterDto Model) : ICommand<ApiResponse<object>>;
}