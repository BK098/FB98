using FB98.Modules.Identity.Application.Authentication.ForgotPassword;
using FB98.Modules.Identity.Application.Authentication.Login;
using FB98.Modules.Identity.Application.Authentication.Register;
using FB98.Modules.Identity.Application.Authentication.ResetPassword;
using FB98.Modules.Identity.Application.ProfileManagement.ChangePassword;
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
		Task<ApiResponse<object>> RevokeTokenAsync(string userId);	
	}
}
