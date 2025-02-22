using FB98.Modules.Warehouse.Application.Abstractions;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.GetStock
{
	internal sealed class GetStockQueryHandler : IQueryHandler<GetStockQuery, ApiResult<GetStockResponse>>
	{
		private readonly ILogger<GetStockQueryHandler> _logger;
		private readonly IInventoryRepository _inventoryRepository;

		public GetStockQueryHandler(
			ILogger<GetStockQueryHandler> logger,
			IInventoryRepository inventoryRepository)
		{
			_logger = logger;
			_inventoryRepository = inventoryRepository;
		}

		public async Task<ApiResult<GetStockResponse>> Handle(GetStockQuery request, CancellationToken cancellationToken)
		{
			var productId = request.ProductId;
			try
			{
				var stock = await _inventoryRepository.GetStock(productId);
				var response = new GetStockResponse
				{
					ProductId = productId,
					Quantity = stock!.Quantity,
					IsLimited = stock.IsLimited
				};
				return ApiResponseBuilder.Success(response, statusCode: 200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get stock");
				return ApiResponseBuilder.Error<GetStockResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
