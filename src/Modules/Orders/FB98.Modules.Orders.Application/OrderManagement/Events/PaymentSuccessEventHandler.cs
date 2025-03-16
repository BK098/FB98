using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Orders.Application.OrderManagement.Events
{
	public class PaymentSuccessEventHandler : IConsumer<PaymentSuccessEvent>
	{
		private readonly ILogger<PaymentSuccessEventHandler> _logger;
		private readonly IOrderRepository _orderRepository;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentSuccessEventHandler(
			IOrderRepository orderRepository,
			ILogger<PaymentSuccessEventHandler> logger,
			IUnitOfWork unitOfWork)
		{
			_orderRepository = orderRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
		{
			try
			{
				var order = await _orderRepository.GetByIdAsync(context.Message.OrderId);
				if (order == null)
				{
					_logger.LogError("Notfound");
					return;
				}

				if (order.OrderStatusId != OrderStatusConstants.Pending)
				{
					_logger.LogError("InvalidState");
					return;
				}

				var previousStatus = order.OrderStatusId;
				order.OrderStatusId = OrderStatusConstants.Confirmed;
				order.SetUpdatedAt();

				var orderStatusHistory = new OrderStatusHistory
				{
					OrderId = order.Id,
					OldStatusId = previousStatus,
					NewStatusId = OrderStatusConstants.Confirmed
				};
				orderStatusHistory.SetCreatedAt();
				_unitOfWork.Entry(order, EntityState.Modified);
				_unitOfWork.Entry(orderStatusHistory, EntityState.Added);
				await _unitOfWork.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}
		}
	}
}