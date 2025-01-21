using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.Authentication.Login
{
	public record LoginCommand(LoginDto Model): ICommand<ApiResponse<LoginResponseDto>>;
}