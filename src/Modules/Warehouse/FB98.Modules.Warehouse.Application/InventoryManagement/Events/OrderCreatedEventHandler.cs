using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public class OrderCreatedEventHandler : IConsumer<OrderCreatedEvent>
	{
		private readonly IInventoryRepository _inventoryRepository;
		private readonly ILogger<PaymentSuccessEventHandler> _logger;

		public OrderCreatedEventHandler(
			IInventoryRepository inventoryRepository,
			ILogger<PaymentSuccessEventHandler> logger)
		{
			_inventoryRepository = inventoryRepository;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<OrderCreatedEvent> context)
		{
			try
			{
				foreach (var item in context.Message.StockItems)
				{
					await _inventoryRepository.ReserveStock(context.Message.OrderId, item.ProductId, item.Quantity);
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to reserve stock");
			}
		}
	}
}