using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public sealed class ProductDeletedEventHandler : IConsumer<ProductDeletedEvent>
	{
		private readonly IInventoryRepository _inventoryRepository;

		public ProductDeletedEventHandler(IInventoryRepository inventoryRepository)
		{
			_inventoryRepository = inventoryRepository;
		}

		public async Task Consume(ConsumeContext<ProductDeletedEvent> context)
		{
			var model = context.Message;
			var stockExists = await _inventoryRepository.Exists(model.ProductId);
			if (stockExists)
			{
				await _inventoryRepository.RemoveProduct(model.ProductId);
			}
		}
	}
}