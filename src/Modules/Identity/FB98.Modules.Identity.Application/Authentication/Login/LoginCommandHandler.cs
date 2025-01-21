using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Modules.Identity.Application.Share.Services;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.Identity.Application.Authentication.Login
{
	public class LoginCommandHandler : ICommandHandler<LoginCommand, ApiResponse<LoginResponseDto>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<LoginCommandHandler> _logger;
		private readonly IValidator<LoginDto> _validator;
		private readonly ILocalizedMessageService _localizedMessage;
		private readonly ITokenService _tokenService;

		public LoginCommandHandler(
			UserManager<AppUser> userManager,
			ILogger<LoginCommandHandler> logger,
			IValidator<LoginDto> validator,
			ILocalizedMessageService localizedMessage,
			ITokenService tokenService)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessage = localizedMessage;
			_tokenService = tokenService;
		}
		public async Task<ApiResponse<LoginResponseDto>> Handle(LoginCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<LoginResponseDto>(validationResult.Errors, _localizedMessage.GetLocalizedMessage("ValidationFailed"));
				}

				var user = await _userManager.FindByEmailAsync(model.Email!);
				if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password!))
				{
					return ApiResponseBuilder.Error<LoginResponseDto>(_localizedMessage.GetLocalizedMessage("InvalidLogin"), statusCode: 401);
				}

				var accessToken = _tokenService.GenerateJwtToken(user);
				var refreshToken = _tokenService.GenerateRefreshToken();

				user.RefreshToken = refreshToken;
				user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success(new LoginResponseDto
				{
					Token = accessToken,
					Expiration = DateTime.UtcNow.AddMinutes(30)
				}, _localizedMessage.GetLocalizedMessage("LoginSuccess"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred during login");
				return ApiResponseBuilder.Error<LoginResponseDto>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
