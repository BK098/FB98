using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events.Base;
using FB98.Shared.Abstractions.Events.Products;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public class ProductCreatedEventHandler : IEventHandler<ProductCreatedEvent>
	{
		private readonly IInventoryRepository _inventoryRepository;

		public ProductCreatedEventHandler(IInventoryRepository inventoryRepository)
		{
			_inventoryRepository = inventoryRepository;
		}

		public async Task HandleAsync(ProductCreatedEvent notification)
		{
			var stockExists = await _inventoryRepository.Exists(notification.ProductId);
			if (!stockExists)
			{
				await _inventoryRepository.AddStockAsync(notification.ProductId, notification.Quantity);
			}
		}
	}
}
