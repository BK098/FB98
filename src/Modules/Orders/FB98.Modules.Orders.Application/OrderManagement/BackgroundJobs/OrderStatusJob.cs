using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Orders.Application.OrderManagement.BackgroundJobs
{
	public sealed class OrderStatusJob : IHostedService, IDisposable
	{
		private readonly ILogger<OrderStatusJob> _logger;
		private readonly IServiceScopeFactory _spoceFactory;
		private Timer? _timer;

		public OrderStatusJob(
			ILogger<OrderStatusJob> logger,
			IServiceScopeFactory spoceFactory)
		{
			_logger = logger;
			_spoceFactory = spoceFactory;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(CheckOrderStatus, null, TimeSpan.Zero, TimeSpan.FromMinutes(10));
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		private async void CheckOrderStatus(object? state)
		{
			try
			{
				if (false)
				{
					using var scope = _spoceFactory.CreateScope();
					var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
					var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
					var now = DateTime.UtcNow;
					var expiredOrders = await orderRepository.GetOrdersByStatusAndTimeAsync(OrderStatusConstants.Created, now.AddMinutes(-7));
					foreach (var order in expiredOrders)
					{
						var orderStatusHistory = new OrderStatusHistory
						{
							OrderId = order.Id,
							OldStatusId = order.OrderStatusId,
							NewStatusId = OrderStatusConstants.Expired
						};
						orderStatusHistory.SetCreatedAt();
						unitOfWork.Entry(orderStatusHistory, EntityState.Added);
						order.OrderStatusId = OrderStatusConstants.Expired;
					}

					var canceledOrders = await orderRepository.GetOrdersByStatusAndTimeAsync(OrderStatusConstants.Confirmed, now.AddDays(-7));
					foreach (var order in canceledOrders)
					{
						var orderStatusHistory = new OrderStatusHistory
						{
							OrderId = order.Id,
							OldStatusId = order.OrderStatusId,
							NewStatusId = OrderStatusConstants.Canceled
						};
						orderStatusHistory.SetCreatedAt();
						unitOfWork.Entry(orderStatusHistory, EntityState.Added);
						order.OrderStatusId = OrderStatusConstants.Canceled;
					}
					await unitOfWork.SaveChangesAsync();
				}
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}
		}
	}
}