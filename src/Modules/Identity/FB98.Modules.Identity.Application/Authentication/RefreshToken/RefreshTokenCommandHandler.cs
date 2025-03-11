using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;

namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, ApiResult<TokenResponse>>
	{
		private readonly IConfiguration _configuration;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<RefreshTokenCommandHandler> _logger;
		private readonly ITokenService _tokenService;
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly UserManager<AppUser> _userManager;

		public RefreshTokenCommandHandler(
			UserManager<AppUser> userManager,
			IConfiguration configuration,
			ILogger<RefreshTokenCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			ITokenService tokenService,
			ITokenStoreRepository tokenStoreRepository)
		{
			_userManager = userManager;
			_configuration = configuration;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_tokenService = tokenService;
			_tokenStoreRepository = tokenStoreRepository;
		}

		public async Task<ApiResult<TokenResponse>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var refreshToken = await _tokenStoreRepository.GetByTokenAsync(request.RefreshToken);
				if (refreshToken == null)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("InvalidRefreshToken"));
				}

				var user = await _userManager.FindByIdAsync(refreshToken.UserId.ToString());
				if (user == null)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("UserNotFound"), 404);
				}

				// Kiểm tra xem refresh token có hợp lệ không
				if (refreshToken.IsRevoked)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("RevokedToken"), 403);
				}

				if (refreshToken.ExpiresAt <= DateTime.UtcNow)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("ExpiredRefreshToken"));
				}

				var newToken = await _tokenService.GenerateAccessToken(user);
				var newRefreshToken = _tokenService.GenerateRefreshToken();
				refreshToken.CreatedAt = DateTime.UtcNow;
				refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
				refreshToken.Token = newRefreshToken;
				await _tokenStoreRepository.UpdateAsync(refreshToken);
				await _userManager.UpdateAsync(user);

				var tokenResponse = new TokenResponse
				{
					Token = newToken,
					RefreshToken = refreshToken.Token
				};
				return ApiResponseBuilder.Success(tokenResponse, _localizedMessageService.GetLocalizedMessage("TokenRefreshed"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred refresh token");
				return ApiResponseBuilder.Error<TokenResponse>("An unexpected error occurred", 500);
			}
		}
	}
}