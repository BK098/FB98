using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.Domain.Entities;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeAllDeviceToken
{
	internal class RevokeAllDeviceTokenCommandHandler : ICommandHandler<RevokeAllDeviceTokenCommand, ApiResponse<object>>
	{
		private readonly ILogger<RevokeAllDeviceTokenCommandHandler> _logger;
		private readonly UserManager<AppUser> _userManager;
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly ILocalizedMessageService _localizedMessage;
		public RevokeAllDeviceTokenCommandHandler(
			ILogger<RevokeAllDeviceTokenCommandHandler> logger,
			UserManager<AppUser> userManager,
			ITokenStoreRepository tokenStoreRepository,
			ILocalizedMessageService localizedMessage)
		{
			_logger = logger;
			_userManager = userManager;
			_tokenStoreRepository = tokenStoreRepository;
			_localizedMessage = localizedMessage;
		}
		public async Task<ApiResponse<object>> Handle(RevokeAllDeviceTokenCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var user = await _userManager.FindByIdAsync(request.UserId.ToString());
				if (user == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("UserNotFound"), statusCode: 404);
				}
				await _tokenStoreRepository.RevokeAllByUserIdAsync(user!.Id);
				await _userManager.UpdateAsync(user);

				return ApiResponseBuilder.Success<object>("", _localizedMessage.GetLocalizedMessage("TokenRevoked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: Revoke All Device Token");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}