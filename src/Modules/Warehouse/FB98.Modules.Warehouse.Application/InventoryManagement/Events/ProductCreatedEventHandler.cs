using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public class ProductCreatedEventHandler : IConsumer<ProductCreatedEvent>
	{
		private readonly IInventoryRepository _inventoryRepository;

		public ProductCreatedEventHandler(IInventoryRepository inventoryRepository)
		{
			_inventoryRepository = inventoryRepository;
		}

		public async Task Consume(ConsumeContext<ProductCreatedEvent> context)
		{
			var model = context.Message;
			var stockExists = await _inventoryRepository.Exists(model.ProductId);
			if (!stockExists)
			{
				await _inventoryRepository.AddStockAsync(model.ProductId, model.Quantity, model.IsLimited);
			}
		}
	}
}
