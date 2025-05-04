using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Infrastructure.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Tickets.Application.SeatManagement.BackgroundJobs
{
	public sealed class SeatUnlockJob : IHostedService, IDisposable
	{
		private readonly ILogger<SeatUnlockJob> _logger;
		private readonly IHubContext<SeatHub> _seatHubContext;
		private readonly IServiceProvider _serviceProvider;
		private Timer? _timer;

		public SeatUnlockJob(
			IServiceProvider serviceProvider,
			IHubContext<SeatHub> seatHubContext,
			ILogger<SeatUnlockJob> logger)
		{
			_serviceProvider = serviceProvider;
			_seatHubContext = seatHubContext;
			_logger = logger;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var taskPeriod = TimeSpan.FromSeconds(10);
			_timer = new Timer(CleanupExpiredLockSeats, null, TimeSpan.Zero, taskPeriod);

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Dispose();
			return Task.CompletedTask;
		}

		private async void CleanupExpiredLockSeats(object? state)
		{
			try
			{
				using var scope = _serviceProvider.CreateScope();
				var repository = scope.ServiceProvider.GetRequiredService<IBookingSeatLockRepository>();

				var affectedShow = await repository.CleanupExpiredLocks();

				if (affectedShow != null)
				{
					foreach (var show in affectedShow)
					{
						await _seatHubContext.Clients.Group(show.ShowId.ToString()).SendAsync("SeatsStatusChanged", show.ShowId);
						//await _notificationHubContext.Clients.User(show.CustomerId.ToString()).SendAsync("SendSeatUnlocks", "Ghế bạn giữ đã bị mở khóa");

						_logger.LogInformation("Unlocks Seats at show {ShowId} completed successfully at {Time}", show, DateTime.UtcNow);
					}
				}
			}
			catch (Exception ex)
			{
				_logger.LogWarning(ex.ToString());
			}
		}
	}
}