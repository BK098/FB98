using FB98.Modules.Payments.Application.Abstractions;

namespace FB98.Modules.Payments.Application.CouponManagement.Delete
{
	internal sealed class DeleteCouponCommandHandler : ICommandHandler<DeleteCouponCommand, ApiResult<object>>
	{
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteCouponCommandHandler> _logger;

		public DeleteCouponCommandHandler(
			ILogger<DeleteCouponCommandHandler> logger,
			ICouponRepository couponRepository,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_couponRepository = couponRepository;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<object>> Handle(DeleteCouponCommand request, CancellationToken cancellationToken)
		{
			try
			{
				var entity = await _couponRepository.GetByIdAsync(request.Id);
				if (entity == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_couponRepository.Delete(entity);
				return ApiResponseBuilder.Success<object>(entity.Id, _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while delete coupon");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}