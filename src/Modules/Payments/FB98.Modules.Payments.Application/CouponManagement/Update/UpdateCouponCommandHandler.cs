using AutoMapper;
using FB98.Modules.Payments.Application.Abstractions;

namespace FB98.Modules.Payments.Application.CouponManagement.Update
{
	internal sealed class UpdateCouponCommandHandler : ICommandHandler<UpdateCouponCommand, ApiResult<object>>
	{
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateCouponCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IValidator<UpdateCouponDto> _validator;

		public UpdateCouponCommandHandler(
			ILogger<UpdateCouponCommandHandler> logger,
			ICouponRepository couponRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			IValidator<UpdateCouponDto> validator)
		{
			_logger = logger;
			_couponRepository = couponRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(UpdateCouponCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var coupon = await _couponRepository.GetByIdAsync(request.Id);
				if (coupon == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var normalizedCode = model.Code!.Normalize().ToUpper().Trim();
				if (!string.IsNullOrWhiteSpace(model.Code))
				{
					if (!normalizedCode.Equals(coupon.Code, StringComparison.Ordinal))
					{
						if (await _couponRepository.IsCouponExisted(normalizedCode))
						{
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
						}

						coupon.Code = normalizedCode;
					}
				}
				_mapper.Map(model, coupon);
				coupon.Code = normalizedCode;
				coupon.StartDate = model.StartDate!.Value.ToUniversalTime();
				coupon.EndDate = model.EndDate!.Value.ToUniversalTime();
				coupon.SetUpdatedAt();

				_couponRepository.Update(coupon);

				return ApiResponseBuilder.Success<object>(coupon.Id, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update coupon");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}