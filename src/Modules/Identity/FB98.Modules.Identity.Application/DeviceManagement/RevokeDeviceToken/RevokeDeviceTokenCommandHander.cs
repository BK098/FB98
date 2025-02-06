using FB98.Modules.Identity.Application.Abtractions;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeDeviceToken
{
	internal class RevokeDeviceTokenCommandHander : ICommandHandler<RevokeDeviceTokenCommand, ApiResponse<object>>
	{
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly ILogger<RevokeDeviceTokenCommandHander> _logger;
		private readonly ILocalizedMessageService _localizedMessage;
		public RevokeDeviceTokenCommandHander(ITokenStoreRepository tokenStoreRepository,
			ILogger<RevokeDeviceTokenCommandHander> logger,
			ILocalizedMessageService localizedMessage)
		{
			_tokenStoreRepository = tokenStoreRepository;
			_logger = logger;
			_localizedMessage = localizedMessage;
		}
		public async Task<ApiResponse<object>> Handle(RevokeDeviceTokenCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var tokenStore = await _tokenStoreRepository.GetByDeviceIdAsync(model.DeviceId, model.UserId);
				if (tokenStore == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessage.GetLocalizedMessage("DeviceOrTokenNotFound"), statusCode: 404);
				}
				tokenStore.IsRevoked = true;
				await _tokenStoreRepository.UpdateAsync(tokenStore);

				return ApiResponseBuilder.Success<object>("", _localizedMessage.GetLocalizedMessage("DeviceTokenRevoked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: Revoke Device Token");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}