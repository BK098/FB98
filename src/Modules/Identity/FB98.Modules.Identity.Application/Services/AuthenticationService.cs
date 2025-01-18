using FB98.Modules.Identity.Application.Entities;
using FB98.Modules.Identity.Application.Models;
using FB98.Shared.Abstractions.Responses;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FB98.Modules.Identity.Application.Services
{
	public class AuthenticationService : IAuthenticationService
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly ILogger<Microsoft.AspNetCore.Authentication.AuthenticationService> _logger;
		private readonly IValidator<LoginDto> _loginDtoValidator;
		private readonly IValidator<RegisterDto> _registerDtoValidator;

		public AuthenticationService(UserManager<AppUser> userManager,
			IConfiguration configuration,
			ILogger<Microsoft.AspNetCore.Authentication.AuthenticationService> logger,
			IValidator<LoginDto> loginDtoValidator,
			IValidator<RegisterDto> registerDtoValidator)
		{
			_userManager = userManager;
			_configuration = configuration;
			_logger = logger;
			_loginDtoValidator = loginDtoValidator;
			_registerDtoValidator = registerDtoValidator;
		}
		public async Task<ApiResponse<LoginResponseDto>> LoginAsync(LoginDto model)
		{
			try
			{
				var validationResult = await _loginDtoValidator.ValidateAsync(model);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<LoginResponseDto>(validationResult.Errors, "Validation failed");
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
				}, "Login successful");
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
				// Validate dữ liệu đầu vào
				var validationResult = await _registerDtoValidator.ValidateAsync(model);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors, "Validation failed");
				}

				// Kiểm tra người dùng đã tồn tại
				var existingUser = await _userManager.FindByEmailAsync(model.Email);
				if (existingUser != null)
				{
					return ApiResponseBuilder.Error<object>("Email already exists", statusCode: 400);
				}

				// Tạo người dùng mới
				var user = new AppUser
				{
					UserName = model.Email,
					Email = model.Email
				};

				var result = await _userManager.CreateAsync(user, model.Password);
				if (!result.Succeeded)
				{
					return ApiResponseBuilder.Error<object>("Failed to create user",
						errors: result.Errors.ToDictionary(
							e => e.Code,
							e => new List<object> { e.Description }
						),
						statusCode: 400);
				}

				// Thành công
				return ApiResponseBuilder.Success<object>("", "User registered successfully");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred during registration");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
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
	}
}
