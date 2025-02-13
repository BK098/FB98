using FB98.Modules.Warehouse.Application.Abstractions;
using MediatR;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.CreateInventory
{
	internal sealed class CreateInventoryCommandHandler : ICommandHandler<CreateInventoryCommand, ApiResponse<Unit>>
	{
		private readonly ILogger<CreateInventoryCommandHandler> _logger;
		private readonly IInventoryRepository _inventoryRepository;
		private readonly IValidator<CreateInventoryDto> _validator;
		public CreateInventoryCommandHandler(
			ILogger<CreateInventoryCommandHandler> logger,
			IInventoryRepository inventoryRepository,
			IValidator<CreateInventoryDto> validator)
		{
			_logger = logger;
			_inventoryRepository = inventoryRepository;
			_validator = validator;
		}
		public async Task<ApiResponse<Unit>> Handle(CreateInventoryCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<Unit>(validationResult.Errors);
				}

				await _inventoryRepository.ReduceStock(model.ProductId!.Value, model.InitialStock!.Value);
				return ApiResponseBuilder.Success(Unit.Value, "", statusCode: 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while reduce stock");
				return ApiResponseBuilder.Error<Unit>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
