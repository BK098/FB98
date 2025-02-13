using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events.Base;
using FB98.Shared.Abstractions.Events.Products;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public sealed class ProductDeletedEventHandler : IEventHandler<ProductDeletedEvent>
	{
		private readonly IInventoryRepository _inventoryRepository;

		public ProductDeletedEventHandler(IInventoryRepository inventoryRepository)
		{
			_inventoryRepository = inventoryRepository;
		}

		public async Task HandleAsync(ProductDeletedEvent notification)
		{
			var stockExists = await _inventoryRepository.Exists(notification.ProductId);
			if (stockExists)
			{
				await _inventoryRepository.RemoveProduct(notification.ProductId);
			}
		}
	}
}
