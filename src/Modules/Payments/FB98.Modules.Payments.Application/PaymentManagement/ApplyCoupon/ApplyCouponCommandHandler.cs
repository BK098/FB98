using FB98.Modules.Payments.Application.Abstractions;

namespace FB98.Modules.Payments.Application.PaymentManagement.ApplyCoupon
{
	internal sealed class ApplyCouponCommandHandler : ICommandHandler<ApplyCouponCommand, ApiResult<ApplyCouponResponse>>
	{
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<ApplyCouponCommandHandler> _logger;
		private readonly IValidator<ApplyCouponDto> _validator;

		public ApplyCouponCommandHandler(
			ILogger<ApplyCouponCommandHandler> logger,
			ICouponRepository couponRepository,
			ILocalizedMessageService localizedMessageService,
			IValidator<ApplyCouponDto> validator)
		{
			_logger = logger;
			_couponRepository = couponRepository;
			_localizedMessageService = localizedMessageService;
			_validator = validator;
		}

		public async Task<ApiResult<ApplyCouponResponse>> Handle(ApplyCouponCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var now = DateTime.UtcNow;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<ApplyCouponResponse>(validationResult.Errors);
				}
				var normalizedCode = model.CouponCode!.Normalize().ToUpper().Trim();

				var coupon = await _couponRepository.GetValidCouponAsync(normalizedCode, model.Amount!.Value, now);
				if (coupon == null)
				{
					return ApiResponseBuilder.Error<ApplyCouponResponse>(_localizedMessageService.GetLocalizedMessage("Invalid"));
				}

				var discount = coupon.CalculateDiscount(model.Amount!.Value);
				var after = model.Amount.Value - discount;

				var response = new ApplyCouponResponse
				{
					Code = model.CouponCode,
					DiscountAmount = after < 0 ? 0 : after,
					DiscountApply = discount
				};
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("CouponApplied"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error apply coupon");
				return ApiResponseBuilder.Error<ApplyCouponResponse>("An unexpected error occurred", 500);
			}
		}
	}
}