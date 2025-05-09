using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Orders.Application.OrderManagement.Events
{
	public class PaymentCreatedEventHandler : IConsumer<PaymentCreatedEvent>
	{
		private readonly ILogger<PaymentCreatedEventHandler> _logger;
		private readonly IOrderRepository _orderRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentCreatedEventHandler(IOrderRepository orderRepository, IUnitOfWork unitOfWork, ILogger<PaymentCreatedEventHandler> logger)
		{
			_orderRepository = orderRepository;
			_unitOfWork = unitOfWork;
			_logger = logger;
		}

		public async Task Consume(ConsumeContext<PaymentCreatedEvent> context)
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

				var order = await _orderRepository.GetByIdAsync(orderId);
				if (order == null)
				{
					_logger.LogError("Order not found: {OrderId}", orderId);
					return;
				}

				if (order.OrderStatusId != OrderStatusConstants.Created)
				{
					_logger.LogError("Order is not in Created state: {OrderId}", orderId);
					return;
				}

				var previousStatus = order.OrderStatusId;
				order.OrderStatusId = OrderStatusConstants.Pending;
				order.SetUpdatedAt();

				var orderStatusHistory = new OrderStatusHistory
				{
					OrderId = order.Id,
					OldStatusId = previousStatus,
					NewStatusId = OrderStatusConstants.Pending
				};
				orderStatusHistory.SetCreatedAt();
				_unitOfWork.Entry(order, EntityState.Modified);
				_unitOfWork.Entry(orderStatusHistory, EntityState.Added);
				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Order status updated to Pending: {OrderId}", orderId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing PaymentCreatedEvent");
			}
		}
	}
}