using AutoMapper;
using FB98.Modules.Orders.Application.Abstractions;

namespace FB98.Modules.Orders.Application.OrderManagement.GetDetail
{
	internal class GetDetailOrderQueryHandler : IQueryHandler<GetDetailOrderQuery, ApiResult<GetDetailOrderResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailOrderQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IOrderRepository _orderRepository;

		public GetDetailOrderQueryHandler(
			IOrderRepository orderRepository,
			IMapper mapper,
			ILogger<GetDetailOrderQueryHandler> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_orderRepository = orderRepository;
			_mapper = mapper;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<GetDetailOrderResponse>> Handle(GetDetailOrderQuery request, CancellationToken cancellationToken)
		{
			var orderId = request.OrderId;
			try
			{
				var order = await _orderRepository.GetByIdAsync(orderId);
				if (order is null)
				{
					return ApiResponseBuilder.Error<GetDetailOrderResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailOrderResponse>(order);
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail order");
				return ApiResponseBuilder.Error<GetDetailOrderResponse>("An unexpected error occurred", 500);
			}
		}
	}
}