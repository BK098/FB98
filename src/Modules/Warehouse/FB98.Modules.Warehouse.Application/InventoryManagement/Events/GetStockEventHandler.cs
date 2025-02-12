using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events.Base;
using FB98.Shared.Abstractions.Events.Products;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public sealed class GetStockQueryHandler : IEventHandler<GetStockEvent>
	{
		private readonly IInventoryRepository _inventoryRepository;
		private readonly IEventDispatcher _eventDispatcher;

		public GetStockQueryHandler(IInventoryRepository inventoryRepository, IEventDispatcher eventDispatcher)
		{
			_inventoryRepository = inventoryRepository;
			_eventDispatcher = eventDispatcher;
		}

		public async Task HandleAsync(GetStockEvent notification)
		{
			var stock = await _inventoryRepository.GetStock(notification.ProductId);
			Console.WriteLine($"[Inventory] GetStockQueryHandler - Sending StockResponseEvent for ProductId: {notification.ProductId} with Stock: {stock}");
			await _eventDispatcher.PublishAsync(new StockResponseEvent(notification.ProductId, stock));
		}
	}
}
