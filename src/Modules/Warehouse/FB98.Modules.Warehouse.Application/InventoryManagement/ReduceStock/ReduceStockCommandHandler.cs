using FB98.Modules.Warehouse.Application.Abstractions;
using MediatR;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.ReduceStock
{
	internal sealed class ReduceStockCommandHandler : ICommandHandler<ReduceStockCommand, ApiResult<Unit>>
	{
		private readonly ILogger<ReduceStockCommandHandler> _logger;
		private readonly IInventoryRepository _inventoryRepository;
		private readonly IValidator<ReduceStockDto> _validator;
		public ReduceStockCommandHandler(
			ILogger<ReduceStockCommandHandler> logger,
			IInventoryRepository inventoryRepository,
			IValidator<ReduceStockDto> validator)
		{
			_logger = logger;
			_inventoryRepository = inventoryRepository;
			_validator = validator;
		}
		public async Task<ApiResult<Unit>> Handle(ReduceStockCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<Unit>(validationResult.Errors);
				}

				await _inventoryRepository.ReduceStock(model.ProductId!.Value, model.Quantity!.Value);
				return ApiResponseBuilder.Success(Unit.Value, statusCode: 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while reduce stock");
				return ApiResponseBuilder.Error<Unit>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
