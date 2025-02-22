using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Orders.Application.OrderManagement.BackgroundJobs
{
	public sealed class OrderStatusJob : IHostedService, IDisposable
	{
		private readonly ILogger<OrderStatusJob> _logger;
		private Timer? _timer;
		private readonly IServiceScopeFactory _spoceFactory;

		public OrderStatusJob(
			ILogger<OrderStatusJob> logger,
			IServiceScopeFactory spoceFactory)
		{
			_logger = logger;
			_spoceFactory = spoceFactory;
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

		public void Dispose()
		{
			_timer?.Dispose();
		}

		private async void CheckOrderStatus(object? state)
		{
			try
			{
				using var scope = _spoceFactory.CreateScope();
				var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
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
					order.StatusHistories.Add(orderStatusHistory);
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
					order.StatusHistories.Add(orderStatusHistory);
					order.OrderStatusId = OrderStatusConstants.Canceled;
				}

				await orderRepository.SaveChangesAsync();
			}
			catch (Exception ex)
			{
				throw; // TODO handle exception
			}
		}
	}
}