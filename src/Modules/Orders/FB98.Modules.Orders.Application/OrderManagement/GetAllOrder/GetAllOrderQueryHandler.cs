using AutoMapper;
using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Orders.Application.OrderManagement.GetAllOrder
{
	internal class GetAllOrderQueryHandler : IQueryHandler<GetAllOrderQuery, ApiResult<PaginatedResult<GetAllOrderResponse>>>
	{
		private readonly List<string> _allowedProperties = ["UserId"];
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllOrderQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderRepository _orderRepository;

		public GetAllOrderQueryHandler(
			ILogger<GetAllOrderQueryHandler> logger,
			IOrderRepository orderRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper)
		{
			_logger = logger;
			_orderRepository = orderRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllOrderResponse>>> Handle(GetAllOrderQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var entities = _orderRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x => EF.Functions.Unaccent(x.UserId.ToString()!).ToLower().Trim().Contains(search));
				}

				if (!await entities.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllOrderResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending)
					.Include(x => x.OrderStatus)
					.OrderBy(x => x.CreateAt);

				var paginatedResult = await PaginatedResult<Order>.CreateAsync(
					entities,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllOrderResponse>(
					_mapper.Map<List<GetAllOrderResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all category");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllOrderResponse>>("An unexpected error occurred",
					500);
			}
		}
	}
}