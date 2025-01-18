using FB98.Modules.Identity.Application.Entities;
using FB98.Modules.Identity.Application.Models;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Security.Cryptography;
using System.Text;
using System.Web;

namespace FB98.Modules.Identity.Application.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly ILogger<AuthenticationService> _logger;
		private readonly IValidator<LoginDto> _loginDtoValidator;
		private readonly IValidator<RegisterDto> _registerDtoValidator;
		private readonly IValidator<ForgotPasswordDto> _forgotPasswordDtoValidator;
		private readonly IValidator<ResetPasswordDto> _resetPasswordDtoValidator;
		private readonly ILocalizedMessageService _localizedMessage;
		private readonly Shared.Infrastructure.Email.IEmailSender _emailSender;

		public AuthenticationService(UserManager<AppUser> userManager,
			IConfiguration configuration,
			ILogger<AuthenticationService> logger,
			IValidator<LoginDto> loginDtoValidator,
			IValidator<RegisterDto> registerDtoValidator,
			ILocalizedMessageService localizedMessage,
			Shared.Infrastructure.Email.IEmailSender emailSender,
			IValidator<ForgotPasswordDto> forgotPasswordDtoValidator,
			IValidator<ResetPasswordDto> resetPasswordDtoValidator)
		{
			_userManager = userManager;
			_configuration = configuration;
			_logger = logger;
			_loginDtoValidator = loginDtoValidator;
			_registerDtoValidator = registerDtoValidator;
			_localizedMessage = localizedMessage;
			_emailSender = emailSender;
			_forgotPasswordDtoValidator = forgotPasswordDtoValidator;
			_resetPasswordDtoValidator = resetPasswordDtoValidator;
		}
		public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto model)
		{
			try
			{
				var validationResult = await _loginDtoValidator.ValidateAsync(model);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<LoginResponseDto>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}

				var user = await _userManager.FindByEmailAsync(model.Email!);
				if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password!))
				{
					return ApiResponseBuilder.Error<LoginResponseDto>("Invalid email or password", statusCode: 401);
				}

				var token = GenerateJwtToken(user);

				// Trả về phản hồi thành công
				return ApiResponseBuilder.Success(new LoginResponseDto
				{
					Token = token,
					Expiration = DateTime.UtcNow.AddMinutes(60)
				}, _localizedMessage.GetLocalizedMessage("LoginSuccess"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred during login");
				return ApiResponseBuilder.Error<LoginResponseDto>("An unexpected error occurred", statusCode: 500);
			}
		}
		public async Task<ApiResponse<object>> RegisterAsync(RegisterDto model)
		{
			try
			{
				var validationResult = await _registerDtoValidator.ValidateAsync(model);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}

				var existingUser = await _userManager.FindByEmailAsync(model.Email);
				if (existingUser != null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("EmailAlreadyExists"), statusCode: 400);
				}

				var user = new AppUser
				{
					UserName = model.Email,
					Email = model.Email,
					PhoneNumber = model.PhoneNumber,
					Firstname = model.Firstname!,
					Lastname = model.Lastname!,
					Age = (byte)model.Age,
					RefreshToken = default!
				};
				var result = await _userManager.CreateAsync(user, model.Password!);

				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("UserCreationFailed"),
						errors: result.Errors.ToDictionary(
							e => e.Code,
							e => new List<object> { e.Description }
						),
						statusCode: 400);
				}
				return ApiResponseBuilder.Success<object>("", _localizedMessage.GetLocalizedMessage("AccountCreatedSuccessfully"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred during registration");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
		public async Task<ApiResponse<TokenResponseDto>> RefreshTokenAsync(RefreshTokenDto model)
		{
			try
			{
				var principal = GetPrincipalFromExpiredToken(model.Token);
				if (principal == null)
				{
					return ApiResponseBuilder.Error<TokenResponseDto>(_localizedMessage.GetLocalizedMessage("InvalidToken"), statusCode: 400);
				}

				var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
				var user = await _userManager.FindByIdAsync(userId!);
				if (user == null || user.RefreshToken != model.RefreshToken || user.RefreshTokenExpiryTime <= DateTime.UtcNow)
				{
					return ApiResponseBuilder.Error<TokenResponseDto>(_localizedMessage.GetLocalizedMessage("InvalidRefreshToken"), statusCode: 400);
				}

				var newToken = GenerateJwtToken(user);
				var newRefreshToken = GenerateRefreshToken();

				user.RefreshToken = newRefreshToken;
				user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success(new TokenResponseDto
				{
					Token = newToken,
					RefreshToken = newRefreshToken
				}, _localizedMessage.GetLocalizedMessage("TokenRefreshed"));
			}
			catch (Exception)
			{
				throw;
			}
		}
		public async Task<ApiResponse<object>> ForgotPasswordAsync(ForgotPasswordDto model)
		{
			try
			{
				var validationResult = await _forgotPasswordDtoValidator.ValidateAsync(model);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}
				var user = await _userManager.FindByEmailAsync(model.Email!);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>("User not found", statusCode: 404);
				}

				var token = await _userManager.GeneratePasswordResetTokenAsync(user);
				var encodedToken = HttpUtility.UrlEncode(token);
				Console.WriteLine($"\x1b[91m \x1b[4m {encodedToken} \x1b[24m \x1b[39m");
				var resetLink = $"{_configuration["FrontendBaseUrl"]}/reset-password?token={HttpUtility.UrlEncode(token)}&email={HttpUtility.UrlEncode(model.Email)}";

				await _emailSender.SendEmailAsync(user.Email!, "Reset Password", resetLink);

				return ApiResponseBuilder.Success<object>("", "Password reset link sent to email");
			}
			catch (Exception)
			{

				throw;
			}
		}
		public async Task<ApiResponse<object>> ResetPasswordAsync(ResetPasswordDto model)
		{
			try
			{
				var validationResult = await _resetPasswordDtoValidator.ValidateAsync(model);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}
				var user = await _userManager.FindByEmailAsync(model.Email);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>("User not found", statusCode: 404);
				}
				var decodedToken = HttpUtility.UrlDecode(model.Token);
				var result = await _userManager.ResetPasswordAsync(user, decodedToken!, model.Password!);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>("Failed to reset password", errors: result.Errors.ToDictionary(e => e.Code, e => new List<object> { e.Description }), statusCode: 400);
				}

				return ApiResponseBuilder.Success<object>("", "Password reset successfully");
			}
			catch (Exception)
			{

				throw;
			}

		}
		public async Task<ApiResponse<object>> ChangePasswordAsync(string userId, ChangePasswordDto model)
		{
			try
			{
				// Xác thực người dùng
				var user = await _userManager.FindByIdAsync(userId);
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>("User not found", statusCode: 404);
				}

				// Xác minh mật khẩu cũ
				var passwordCheck = await _userManager.CheckPasswordAsync(user, model.CurrentPassword);
				if (!passwordCheck)
				{
					return ApiResponseBuilder.Error<object>("Current password is incorrect", statusCode: 400);
				}

				// Đổi mật khẩu
				var result = await _userManager.ChangePasswordAsync(user, model.CurrentPassword, model.NewPassword);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>("Failed to change password", errors: result.Errors.ToDictionary(
						e => e.Code, e => new List<object> { e.Description }), statusCode: 400);
				}

				return ApiResponseBuilder.Success<object>(null, "Password changed successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while changing password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}


		//public async Task<ApiResponse<object>> SendOtpAsync(string phoneNumber)
		//{
		//	var otp = new Random().Next(100000, 999999).ToString();
		//	_cache.Set(phoneNumber, otp, TimeSpan.FromMinutes(5)); // Lưu OTP vào cache

		//	// Gửi OTP qua SMS (Twilio/Firebase)
		//	await _smsSender.SendSmsAsync(phoneNumber, $"Your OTP is {otp}");

		//	return ApiResponseBuilder.Success<object>(null, "OTP sent to phone number");
		//}

		//public ApiResponse<object> VerifyOtp(string phoneNumber, string otp)
		//{
		//	var cachedOtp = _cache.Get<string>(phoneNumber);
		//	if (cachedOtp == null || cachedOtp != otp)
		//	{
		//		return ApiResponseBuilder.Error<object>("Invalid or expired OTP", statusCode: 400);
		//	}

		//	return ApiResponseBuilder.Success<object>(null, "OTP verified successfully");
		//}

		private string GenerateJwtToken(AppUser user)
		{
			var claims = new[]
			{
				new Claim(JwtRegisteredClaimNames.Sub, user.Id.ToString()),
				new Claim(JwtRegisteredClaimNames.UniqueName, user.UserName!),
			};

			var key = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!));
			var creds = new SigningCredentials(key, SecurityAlgorithms.HmacSha256);

			var token = new JwtSecurityToken(
				_configuration["Jwt:Issuer"],
				_configuration["Jwt:Audience"],
				claims,
				expires: DateTime.UtcNow.AddMinutes(60),
				signingCredentials: creds);

			return new JwtSecurityTokenHandler().WriteToken(token);
		}
		private string GenerateRefreshToken()
		{
			var randomBytes = new byte[64];
			using var rng = RandomNumberGenerator.Create();
			rng.GetBytes(randomBytes);
			return Convert.ToBase64String(randomBytes);
		}
		private ClaimsPrincipal GetPrincipalFromExpiredToken(string token)
		{
			var tokenValidationParameters = new TokenValidationParameters
			{
				ValidateIssuerSigningKey = true,
				IssuerSigningKey = new SymmetricSecurityKey(Encoding.UTF8.GetBytes(_configuration["Jwt:Key"]!)),
				ValidateIssuer = false,
				ValidateAudience = false,
				ValidateLifetime = false
			};

			var tokenHandler = new JwtSecurityTokenHandler();
			var principal = tokenHandler.ValidateToken(token, tokenValidationParameters, out var securityToken);

			if (securityToken is not JwtSecurityToken jwtSecurityToken ||
				!jwtSecurityToken.Header.Alg.Equals(SecurityAlgorithms.HmacSha256, StringComparison.InvariantCultureIgnoreCase))
			{
				throw new SecurityTokenException("Invalid token");
			}

			return principal;
		}

	}
}
