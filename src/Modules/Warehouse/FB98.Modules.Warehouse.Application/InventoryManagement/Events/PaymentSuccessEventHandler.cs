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
			var model = context.Message;
			try
			{
				await _inventoryRepository.StockDeduct(model.OrderId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get StockDeductedEvent");
			}
		}
	}
}