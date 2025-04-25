using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Payments.Application")]
[assembly: InternalsVisibleTo("FB98.Shared.Infrastructure")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface IUserApi
	{
		[Get("/identity-module/Profile?UserId={model.UserId}&Email={model.Email}&PhoneNumber={model.PhoneNumber}")]
		Task<ApiResult<UserResponse>> GetUserProfile(UserDto model);
	}

	public record UserDto(string? UserId = null, string? Email = null, string? PhoneNumber = null);

	public record UserResponse(string UserId, string Email, string PhoneNumber);
}