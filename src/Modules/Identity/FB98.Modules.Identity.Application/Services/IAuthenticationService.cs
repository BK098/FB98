using FB98.Modules.Identity.Application.Models;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Identity.Application.Services
{
	public interface IAuthenticationService
	{
		Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto model);
		Task<ApiResponse<object>> RegisterAsync(RegisterDto model);
		Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model);
		Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model);
		Task<ApiResponse<object>> ChangePasswordAsync(string userId, ChangePasswordDto model);
	}
}
