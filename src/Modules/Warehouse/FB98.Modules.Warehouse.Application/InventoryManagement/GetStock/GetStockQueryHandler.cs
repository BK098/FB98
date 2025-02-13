
using FB98.Modules.Warehouse.Application.Abstractions;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.GetStock
{
	internal sealed class GetStockQueryHandler : IQueryHandler<GetStockQuery, ApiResponse<int>>
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
		public async Task<ApiResponse<int>> Handle(GetStockQuery request, CancellationToken cancellationToken)
		{
			var productId = request.ProductId;
			try
			{
				var stock = await _inventoryRepository.GetStock(productId);
				return ApiResponseBuilder.Success(stock, statusCode: 200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get stock");
				return ApiResponseBuilder.Error<int>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
