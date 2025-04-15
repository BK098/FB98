using AutoMapper;
using FB98.Modules.Orders.Application.Abstractions;

namespace FB98.Modules.Orders.Application.OrderManagement.GetOrderStatusHistory
{
	public  sealed class GetOrderStatusHistoryQueryHandler : IQueryHandler<GetOrderStatusHistoryQuery, ApiResult<IEnumerable<GetOrderStatusHistoryResponse>>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetOrderStatusHistoryQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderRepository _orderRepository;

		public GetOrderStatusHistoryQueryHandler(
			IOrderRepository orderRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetOrderStatusHistoryQueryHandler> logger,
			IMapper mapper)
		{
			_orderRepository = orderRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<IEnumerable<GetOrderStatusHistoryResponse>>> Handle(GetOrderStatusHistoryQuery request, CancellationToken cancellationToken)
		{
			try
			{
				var history = await _orderRepository.GetOrderStatusHistoryAsync(request.OrderId);
				if (history == null || !history.Any())
				{
					return ApiResponseBuilder.Error<IEnumerable<GetOrderStatusHistoryResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<IEnumerable<GetOrderStatusHistoryResponse>>(history);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get order status history");
				return ApiResponseBuilder.Error<IEnumerable<GetOrderStatusHistoryResponse>>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}