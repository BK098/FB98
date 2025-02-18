using FB98.Modules.Identity.Application.Abtractions;

namespace FB98.Modules.Identity.Application.DeviceManagement.RevokeDeviceToken
{
	internal class RevokeDeviceTokenCommandHander : ICommandHandler<RevokeDeviceTokenCommand, ApiResult<object>>
	{
		private readonly ITokenStoreRepository _tokenStoreRepository;
		private readonly ILogger<RevokeDeviceTokenCommandHander> _logger;
		private readonly ILocalizedMessageService _localizedMessageService;
		public RevokeDeviceTokenCommandHander(ITokenStoreRepository tokenStoreRepository,
			ILogger<RevokeDeviceTokenCommandHander> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_tokenStoreRepository = tokenStoreRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}
		public async Task<ApiResult<object>> Handle(RevokeDeviceTokenCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var tokenStore = await _tokenStoreRepository.GetByDeviceIdAsync(model.DeviceId, model.UserId);
				if (tokenStore == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeviceOrTokenNotFound"), statusCode: 404);
				}
				tokenStore.IsRevoked = true;
				await _tokenStoreRepository.UpdateAsync(tokenStore);

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("DeviceTokenRevoked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "An error occurred: Revoke Device Token");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}