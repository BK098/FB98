using FB98.Modules.Warehouse.Application.Abstractions;
using MediatR;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.AddStock
{
	internal sealed class AddStockCommandHandler : ICommandHandler<AddStockCommand, ApiResponse<Unit>>
	{
		private readonly ILogger<AddStockCommandHandler> _logger;
		private readonly IInventoryRepository _inventoryRepository;
		private readonly IValidator<AddStockDto> _validator;
		public AddStockCommandHandler(
			IInventoryRepository inventoryRepository,
			ILogger<AddStockCommandHandler> logger,
			IValidator<AddStockDto> validator)
		{
			_inventoryRepository = inventoryRepository;
			_logger = logger;
			_validator = validator;
		}
		public async Task<ApiResponse<Unit>> Handle(AddStockCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<Unit>(validationResult.Errors);
				}

				await _inventoryRepository.AddStockAsync(model.ProductId!.Value, model.Quantity!.Value);
				return ApiResponseBuilder.Success(Unit.Value, statusCode: 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while forgot password");
				return ApiResponseBuilder.Error<Unit>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
