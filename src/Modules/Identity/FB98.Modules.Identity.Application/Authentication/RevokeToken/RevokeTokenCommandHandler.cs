using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.Authentication.RevokeToken
{
	internal sealed class RevokeTokenCommandHandler : ICommandHandler<RevokeTokenCommand, ApiResponse<object>>
	{
		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly ILogger<RevokeTokenCommandHandler> _logger;
		public RevokeTokenCommandHandler(
			UserManager<AppUser> userManager,
			ITokenStoreRepository tokenStoreRepository,
			ILogger<RevokeTokenCommandHandler> logger)
		{
			_userManager = userManager;
			_tokenStoreRepository = tokenStoreRepository;
			_logger = logger;
		}
		public async Task<ApiResponse<object>> Handle(RevokeTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(request.UserId.ToString());
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>("User not found", statusCode: 404);
				}
				await _tokenStoreRepository.RevokeAllByUserIdAsync(user!.Id);
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success<object>("", "Token revoked successfully", statusCode: 204);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: Revoke Token");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
