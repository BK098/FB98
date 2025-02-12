using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Application.ProductManagement.Events;
using FB98.Shared.Abstractions.Events.Base;
using FB98.Shared.Abstractions.Events.Products;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetDetail
{
	internal sealed class GetDetailProductQueryHandler : IQueryHandler<GetDetailProductQuery, ApiResponse<GetDetailProductResponse>>
	{
		private readonly ILogger<GetDetailProductQueryHandler> _logger;
		private readonly IProductRepository _productRepository;
		private readonly IMapper _mapper;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IEventDispatcher _eventDispatcher;

		public GetDetailProductQueryHandler(ILogger<GetDetailProductQueryHandler> logger,
			IProductRepository productRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			IEventDispatcher eventDispatcher)
		{
			_logger = logger;
			_productRepository = productRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_eventDispatcher = eventDispatcher;
		}

		public async Task<ApiResponse<GetDetailProductResponse>> Handle(GetDetailProductQuery request, CancellationToken cancellationToken)
		{
			var productId = request.ProductId;
			try
			{
				var product = await _productRepository.GetByIdAsync(productId);
				if (product is null)
				{
					return ApiResponseBuilder.Error<GetDetailProductResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}

#if DEBUG
				Console.WriteLine($"[Catalog] GetDetailProductQueryHandler - Sending GetStockEvent for ProductId: {productId}");
				await _eventDispatcher.PublishAsync(new GetStockEvent(productId));
				Console.WriteLine($"[Catalog] GetDetailProductQueryHandler - Waiting for StockResponseEvent for ProductId: {productId}");
				int remainingQuantity = await StockResponseEventHandler.WaitForStockResponse(productId);
				Console.WriteLine($"[Catalog] GetDetailProductQueryHandler - Received Stock Quantity: {remainingQuantity} for ProductId: {productId}");
#else
				await _eventDispatcher.PublishAsync(new GetStockEvent(productId));
				int remainingQuantity = await StockResponseEventHandler.WaitForStockResponse(productId);
#endif
				var response = _mapper.Map<GetDetailProductResponse>(product);

				response.RemainingQuantity = remainingQuantity;

				return ApiResponseBuilder.Success(response, statusCode: 200);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get product detail");
				return ApiResponseBuilder.Error<GetDetailProductResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
