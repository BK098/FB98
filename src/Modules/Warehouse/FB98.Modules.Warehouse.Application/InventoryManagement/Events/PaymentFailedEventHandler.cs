using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public class PaymentFailedEventHandler : IConsumer<PaymentFailedEvent>
	{
		private readonly IInventoryRepository _inventoryRepository;
		private readonly ILogger<PaymentSuccessEventHandler> _logger;

		public PaymentFailedEventHandler(IInventoryRepository inventoryRepository, ILogger<PaymentSuccessEventHandler> logger)
		{
			_inventoryRepository = inventoryRepository;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<PaymentFailedEvent> context)
		{
			try
			{
				var orderId = context.Message.OrderId!.Value;
				await _inventoryRepository.ReleaseStock(orderId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Failed to release stock.");
			}
		}
	}
}