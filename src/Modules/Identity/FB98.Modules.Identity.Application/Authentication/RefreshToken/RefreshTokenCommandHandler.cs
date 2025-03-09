using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Application.Services;
using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	internal sealed class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, ApiResult<TokenResponse>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly ILogger<RefreshTokenCommandHandler> _logger;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ITokenService _tokenService;
		private readonly ITokenStoreRepository _tokenStoreRepository;

		public RefreshTokenCommandHandler(UserManager<AppUser> userManager,
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
			var model = request.Model;
			try
			{
				var principal = GetPrincipalFromExpiredToken(model.Token);
				if (principal == null)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("InvalidToken"), statusCode: 400);
				}

				var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
				var user = await _userManager.FindByIdAsync(userId!);
				var refreshToken = await _tokenStoreRepository.GetByTokenAsync(model.RefreshToken);

				if (user == null || refreshToken?.Token != model.RefreshToken)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("InvalidRefreshToken"), statusCode: 400);
				}
				if (refreshToken.IsRevoked)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("RevokedToken"), statusCode: 403);
				}
				if (refreshToken.ExpiresAt <= DateTime.UtcNow)
				{
					return ApiResponseBuilder.Error<TokenResponse>(_localizedMessageService.GetLocalizedMessage("ExpiredRefreshToken"), statusCode: 400);
				}

				var newToken = await _tokenService.GenerateAccessToken(user);

				refreshToken.CreatedAt = DateTime.UtcNow;
				refreshToken.ExpiresAt = DateTime.UtcNow.AddDays(7);
				await _tokenStoreRepository.UpdateAsync(refreshToken);
				await _userManager.UpdateAsync(user);

				var tokenResponse = new TokenResponse
				{
					Token = newToken,
					RefreshToken = model.RefreshToken
				};
				return ApiResponseBuilder.Success(tokenResponse, _localizedMessageService.GetLocalizedMessage("TokenRefreshed"), statusCode: 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: refresh token");
				return ApiResponseBuilder.Error<TokenResponse>("An unexpected error occurred", statusCode: 500);
			}
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
