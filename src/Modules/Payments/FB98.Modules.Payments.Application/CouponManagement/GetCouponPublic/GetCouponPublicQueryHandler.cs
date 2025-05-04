using AutoMapper;
using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Application.CouponManagement.GetDetail;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Payments.Application.CouponManagement.GetCouponPublic
{
	internal class GetCouponPublicQueryHandler : IQueryHandler<GetCouponPublicQuery, ApiResult<PaginatedResult<GetCouponPublicResponse>>>
	{
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailCouponQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetCouponPublicQueryHandler(
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

		public async Task<ApiResult<PaginatedResult<GetCouponPublicResponse>>> Handle(GetCouponPublicQuery request, CancellationToken cancellationToken)
		{
			var amount = request.Amount;
			try
			{
				var query = _couponRepository.GetAll()
					.Where(x => !x.IsLimited
								&& x.IsActive
								&& x.StartDate <= DateTime.UtcNow
								&& x.EndDate >= DateTime.UtcNow
								&& amount >= x.MinPaymentAmount);

				var totalItems = await query.CountAsync(cancellationToken);
				if (totalItems == 0)
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetCouponPublicResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var paginatedResult = await PaginatedResult<Coupon>.CreateAsync(
					query,
					1,
					totalItems,
					cancellationToken);

				var response = new PaginatedResult<GetCouponPublicResponse>(
					_mapper.Map<List<GetCouponPublicResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting public coupons");
				return ApiResponseBuilder.Error<PaginatedResult<GetCouponPublicResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}