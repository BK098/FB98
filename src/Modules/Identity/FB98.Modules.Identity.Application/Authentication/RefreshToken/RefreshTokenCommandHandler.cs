using FB98.Modules.Identity.Application.Share.Entities;
using FB98.Modules.Identity.Application.Share.Services;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.Logging;
using Microsoft.IdentityModel.Tokens;
using System.IdentityModel.Tokens.Jwt;
using System.Security.Claims;
using System.Text;

namespace FB98.Modules.Identity.Application.Authentication.RefreshToken
{
	public class RefreshTokenCommandHandler : ICommandHandler<RefreshTokenCommand, ApiResponse<TokenResponseDto>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly IConfiguration _configuration;
		private readonly ILogger<RefreshTokenCommandHandler> _logger;
		private readonly ILocalizedMessageService _localizedMessage;
		private readonly ITokenService _tokenService;

		public RefreshTokenCommandHandler(UserManager<AppUser> userManager,
			IConfiguration configuration,
			ILogger<RefreshTokenCommandHandler> logger,
			ILocalizedMessageService localizedMessage,
			ITokenService tokenService)
		{
			_userManager = userManager;
			_configuration = configuration;
			_logger = logger;
			_localizedMessage = localizedMessage;
			_tokenService = tokenService;
		}
		public async Task<ApiResponse<TokenResponseDto>> Handle(RefreshTokenCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var principal = GetPrincipalFromExpiredToken(model.Token);
				if (principal == null)
				{
					return ApiResponseBuilder.Error<TokenResponseDto>(_localizedMessage.GetLocalizedMessage("InvalidToken"), statusCode: 400);
				}

				var userId = principal.FindFirstValue(ClaimTypes.NameIdentifier);
				var user = await _userManager.FindByIdAsync(userId!);
				if (user == null || user.RefreshToken != model.RefreshToken)
				{
					return ApiResponseBuilder.Error<TokenResponseDto>(_localizedMessage.GetLocalizedMessage("InvalidRefreshToken"), statusCode: 400);
				}
				if (user.IsRevoked)
				{
					return ApiResponseBuilder.Error<TokenResponseDto>(_localizedMessage.GetLocalizedMessage("RevokedToken"), statusCode: 403);
				}
				if (user.RefreshTokenExpiryTime <= DateTime.UtcNow)
				{
					return ApiResponseBuilder.Error<TokenResponseDto>(_localizedMessage.GetLocalizedMessage("ExpiredRefreshToken"), statusCode: 400);
				}

				var newToken = _tokenService.GenerateJwtToken(user);
				var newRefreshToken = _tokenService.GenerateRefreshToken();

				user.RefreshToken = newRefreshToken;
				user.RefreshTokenExpiryTime = DateTime.UtcNow.AddDays(7);

				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success(new TokenResponseDto
				{
					Token = newToken,
					RefreshToken = newRefreshToken
				}, _localizedMessage.GetLocalizedMessage("TokenRefreshed"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while refreshing token");
				return ApiResponseBuilder.Error<TokenResponseDto>("An unexpected error occurred", statusCode: 500);
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
