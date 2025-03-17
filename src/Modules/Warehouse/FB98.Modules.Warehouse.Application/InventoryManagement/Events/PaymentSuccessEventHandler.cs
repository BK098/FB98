using FB98.Modules.Warehouse.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.Events
{
	public sealed class PaymentSuccessEventHandler : IConsumer<PaymentSuccessEvent>
	{
		private readonly ILogger<PaymentSuccessEventHandler> _logger;
		private readonly IInventoryRepository _inventoryRepository;

		public PaymentSuccessEventHandler(
			ILogger<PaymentSuccessEventHandler> logger,
			IInventoryRepository inventoryRepository)
		{
			_logger = logger;
			_inventoryRepository = inventoryRepository;
		}

		public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
		{
			try
			{
				var orderId = context.Message.OrderId;
				if (orderId == null)
				{
					_logger.LogInformation("OrderId is null, skipping order processing.");
					await context.ConsumeCompleted;
					return;
				}
				await _inventoryRepository.StockDeduct(orderId!.Value);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get StockDeductedEvent");
			}
		}
	}
}