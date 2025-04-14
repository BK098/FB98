using AutoMapper;
using FB98.Modules.Payments.Application.Abstractions;
using FB98.Modules.Payments.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Payments.Application.CouponManagement.GetAll
{
	internal sealed class GetAllCouponQueryHandler : IQueryHandler<GetAllCouponQuery, ApiResult<PaginatedResult<GetAllCouponResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Code"];
		private readonly ICouponRepository _couponRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllCouponQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllCouponQueryHandler(
			ILogger<GetAllCouponQueryHandler> logger,
			ICouponRepository couponRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper)
		{
			_logger = logger;
			_couponRepository = couponRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllCouponResponse>>> Handle(GetAllCouponQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var entities = _couponRepository.GetAll().AsNoTracking();

				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x => EF.Functions.Unaccent(x.Code).ToLower().Trim().Contains(search));
				}

				if (!await entities.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllCouponResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<Coupon>.CreateAsync(
					entities,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllCouponResponse>(
					_mapper.Map<List<GetAllCouponResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all coupon");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllCouponResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}
