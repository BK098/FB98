using AutoMapper;
using FB98.Modules.Payments.Application.Abstractions;

namespace FB98.Modules.Payments.Application.CouponManagement.GetDetail
{
	internal sealed class GetDetailCouponQueryHandler : IQueryHandler<GetDetailCouponQuery, ApiResult<GetDetailCouponResponse>>
	{
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailCouponQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailCouponQueryHandler(
			ICouponRepository couponRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetDetailCouponQueryHandler> logger,
			IMapper mapper)
		{
			_couponRepository = couponRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<GetDetailCouponResponse>> Handle(GetDetailCouponQuery request, CancellationToken cancellationToken)
		{
			var couponId = request.CouponId;
			try
			{
				var coupon = await _couponRepository.GetByIdAsync(couponId);
				if (coupon == null)
				{
					return ApiResponseBuilder.Error<GetDetailCouponResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailCouponResponse>(coupon);
				if (coupon.IsDiscountPercentage)
				{
					response.Value = $"{coupon.Value}%";
				}
				else
				{
					response.Value = $"{coupon.Value}";
				}

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create coupon");
				return ApiResponseBuilder.Error<GetDetailCouponResponse>("An unexpected error occurred", 500);
			}
		}
	}
}