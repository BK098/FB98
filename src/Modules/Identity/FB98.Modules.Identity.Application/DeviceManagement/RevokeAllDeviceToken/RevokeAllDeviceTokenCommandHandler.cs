using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Domain.Entities;
using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeAllDeviceToken
{
	internal sealed class RevokeAllDeviceTokenCommandHandler : ICommandHandler<RevokeAllDeviceTokenCommand, ApiResponse<object>>
	{
		private readonly ILogger<RevokeAllDeviceTokenCommandHandler> _logger;
		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		public RevokeAllDeviceTokenCommandHandler(
			ILogger<RevokeAllDeviceTokenCommandHandler> logger,
			UserManager<AppUser> userManager,
			ITokenStoreRepository tokenStoreRepository,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_userManager = userManager;
			_tokenStoreRepository = tokenStoreRepository;
			_localizedMessageService = localizedMessageService;
		}
		public async Task<ApiResponse<object>> Handle(RevokeAllDeviceTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(request.UserId.ToString());
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}
				await _tokenStoreRepository.RevokeAllByUserIdAsync(user!.Id);
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("TokenRevoked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: Revoke All Device Token");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}