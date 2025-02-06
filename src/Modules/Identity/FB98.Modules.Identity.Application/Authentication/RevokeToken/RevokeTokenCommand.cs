using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.Authentication.RevokeToken
{
	public record RevokeTokenCommand(Guid UserId) : ICommand<ApiResponse<object>>;
}
