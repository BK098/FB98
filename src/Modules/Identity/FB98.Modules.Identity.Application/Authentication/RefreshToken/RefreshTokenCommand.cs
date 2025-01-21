using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	public record RefreshTokenCommand(RefreshTokenDto Model) : ICommand<ApiResponse<TokenResponseDto>>;
}
