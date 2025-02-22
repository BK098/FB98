using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.Authentication.Login
{
	internal sealed class LoginCommandHandler : ICommandHandler<LoginCommand, ApiResult<LoginResponse>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ILogger<LoginCommandHandler> _logger;
		private readonly IValidator<LoginDto> _validator;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ITokenService _tokenService;
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly IHttpContextAccessor _httpContextAccessor;

		public LoginCommandHandler(
			UserManager<AppUser> userManager,
			ILogger<LoginCommandHandler> logger,
			IValidator<LoginDto> validator,
			ILocalizedMessageService localizedMessageService,
			ITokenService tokenService,
			ITokenStoreRepository tokenStoreRepository,
			IHttpContextAccessor httpContextAccessor)
		{
			_userManager = userManager;
			_logger = logger;
			_validator = validator;
			_localizedMessageService = localizedMessageService;
			_tokenService = tokenService;
			_tokenStoreRepository = tokenStoreRepository;
			_httpContextAccessor = httpContextAccessor;
		}
		public async Task<ApiResult<LoginResponse>> Handle(LoginCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<LoginResponse>(validationResult.Errors, _localizedMessageService.GetLocalizedMessage("ValidationFailed"));
				}

				var user = await _userManager.FindByEmailAsync(model.Email!);
				if (user == null || !await _userManager.CheckPasswordAsync(user, model.Password!))
				{
					return ApiResponseBuilder.Error<LoginResponse>(_localizedMessageService.GetLocalizedMessage("InvalidLogin"), statusCode: 401);
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
							return ApiResponseBuilder.Error<LoginResponse>(
								_localizedMessageService.GetLocalizedMessage("DeviceAlreadyLoggedIn"),
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

						var loginResponse = new LoginResponse
						{
							Token = accessToken,
							Expiration = DateTime.UtcNow.AddMinutes(30)
						};
						return ApiResponseBuilder.Success(
							loginResponse,
							_localizedMessageService.GetLocalizedMessage("LoginSuccess"));
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

				return ApiResponseBuilder.Success(new LoginResponse
				{
					Token = accessToken,
					RefreshToken = refreshToken,
					Expiration = DateTime.UtcNow.AddMinutes(30)
				}, _localizedMessageService.GetLocalizedMessage("LoginSuccess"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: login");
				return ApiResponseBuilder.Error<LoginResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
