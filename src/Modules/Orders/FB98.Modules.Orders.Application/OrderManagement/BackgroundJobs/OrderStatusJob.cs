using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Orders.Application.OrderManagement.BackgroundJobs
{
	public sealed class OrderStatusJob : IHostedService, IDisposable
	{
		private readonly IConfiguration _configuration;
		private readonly ILogger<OrderStatusJob> _logger;
		private readonly IServiceScopeFactory _spoceFactory;
		private Timer? _timer;

		public OrderStatusJob(ILogger<OrderStatusJob> logger, IServiceScopeFactory spoceFactory, IConfiguration configuration)
		{
			_logger = logger;
			_spoceFactory = spoceFactory;
			_configuration = configuration;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var orderStatusJobEnabled = _configuration.GetValue<bool>("BackGroundJobs:OrdersModule:OrderStatusJob:Enabled");
			var orderStatusJobInterval = _configuration.GetValue<int>("BackGroundJobs:OrdersModule:OrderStatusJob:Interval");
			_logger.LogInformation($"OrderStatusJob is {(orderStatusJobEnabled ? "enabled" : "disabled")}");

			var taskPeriod = TimeSpan.FromSeconds(orderStatusJobInterval);
			if (orderStatusJobEnabled)
			{
				_timer = new Timer(CheckOrderStatus, null, TimeSpan.Zero, taskPeriod);
			}

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
				_logger.LogInformation("OrderStatusJob running at {Time}", DateTime.UtcNow);
				using var scope = _spoceFactory.CreateScope();
				var orderRepository = scope.ServiceProvider.GetRequiredService<IOrderRepository>();
				var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
				var now = DateTime.UtcNow;
				var expiredOrders = await orderRepository.GetOrdersByStatusAndTimeAsync(OrderStatusConstants.Created, now.AddMinutes(-7));
				foreach (var order in expiredOrders)
				{
					_logger.LogInformation("Expiring order {OrderId}", order.Id);
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

				var pendingOrders = await orderRepository.GetOrdersByStatusAndTimeAsync(OrderStatusConstants.Pending, now.AddMinutes(-7));
				foreach (var order in pendingOrders)
				{
					_logger.LogInformation("Expiring order {OrderId}", order.Id);
					var orderStatusHistory = new OrderStatusHistory
					{
						OrderId = order.Id,
						OldStatusId = order.OrderStatusId,
						NewStatusId = OrderStatusConstants.Pending
					};
					orderStatusHistory.SetCreatedAt();
					unitOfWork.Entry(orderStatusHistory, EntityState.Added);
					order.OrderStatusId = OrderStatusConstants.Expired;
				}

				var canceledOrders = await orderRepository.GetOrdersByStatusAndTimeAsync(OrderStatusConstants.Confirmed, now.AddDays(-7));
				foreach (var order in canceledOrders)
				{
					_logger.LogInformation("Cancelling order {OrderId}", order.Id);
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
				_logger.LogInformation("OrderStatusJob completed successfully at {Time}", DateTime.UtcNow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}
		}
	}
}