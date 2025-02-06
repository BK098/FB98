using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.AspNetCore.Http;
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
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public LoginCommandHandler(
			UserManager<AppUser> userManager,
			ILogger<LoginCommandHandler> logger,
			IValidator<LoginDto> validator,
			ILocalizedMessageService localizedMessage,
			ITokenService tokenService,
			ITokenStoreRepository tokenStoreRepository,
			IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessage = localizedMessage;
			_tokenService = tokenService;
			_tokenStoreRepository = tokenStoreRepository;
			_httpContextAccessor = httpContextAccessor;
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

				var accessToken = _tokenService.GenerateAccessToken(user);
				var refreshToken = _tokenService.GenerateRefreshToken();
				var ipAddress = _httpContextAccessor.HttpContext?.Connection.RemoteIpAddress?.ToString() ?? "Unknown";
				var userAgent = _httpContextAccessor.HttpContext?.Request.Headers.UserAgent.ToString() ?? "Unknown";

				if (model.DeviceId is not null)
				{
					var existingToken = await _tokenStoreRepository.GetByDeviceIdAsync((Guid)model.DeviceId, user.Id);
					if (existingToken != null)
					{
						if (!existingToken.IsRevoked)
						{
							return ApiResponseBuilder.Error<LoginResponseDto>(
								_localizedMessage.GetLocalizedMessage("DeviceAlreadyLoggedIn"),
								statusCode: 403
							);
						}
						existingToken.IpAddress = ipAddress!;
						existingToken.UserAgent = userAgent!;
						existingToken.DeviceName = string.IsNullOrEmpty(model.DeviceName) ? "Unknown Device" : model.DeviceName;
						existingToken.Token = _tokenService.GenerateRefreshToken();
						existingToken.CreatedAt = DateTime.UtcNow;
						existingToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
						existingToken.IsRevoked = false;
						await _tokenStoreRepository.UpdateAsync(existingToken);

						return ApiResponseBuilder.Success(new LoginResponseDto
						{
							Token = accessToken,
							Expiration = DateTime.UtcNow.AddMinutes(30)
						}, _localizedMessage.GetLocalizedMessage("LoginSuccess"));
					}
				}
				var tokenStore = new TokenStore
				{
					Id = Guid.NewGuid(),
					Token = refreshToken,
					DeviceId = model.DeviceId ?? Guid.NewGuid(),
					DeviceName = string.IsNullOrEmpty(model.DeviceName) ? "Unknown Device" : model.DeviceName,
					IpAddress = ipAddress!,
					UserAgent = userAgent!,
					CreatedAt = DateTime.UtcNow,
					ExpiresAt = DateTime.UtcNow.AddDays(7),
					IsRevoked = false,
					UserId = user.Id
				};

				await _tokenStoreRepository.AddAsync(tokenStore);
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success(new LoginResponseDto
				{
					Token = accessToken,
					Expiration = DateTime.UtcNow.AddMinutes(30)
				}, _localizedMessage.GetLocalizedMessage("LoginSuccess"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: login");
				return ApiResponseBuilder.Error<LoginResponseDto>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
