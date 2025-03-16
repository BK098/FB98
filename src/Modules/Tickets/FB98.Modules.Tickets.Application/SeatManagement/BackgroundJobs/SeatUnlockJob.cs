using FB98.Modules.Tickets.Application.Abstractions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Tickets.Application.SeatManagement.BackgroundJobs
{
	public class SeatUnlockJob : IHostedService, IDisposable
	{
		private readonly IServiceProvider _serviceProvider;
		private Timer _timer;

		public SeatUnlockJob(IServiceProvider serviceProvider)
		{
			_serviceProvider = serviceProvider;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			_timer = new Timer(async _ =>
			{
				using var scope = _serviceProvider.CreateScope();
				var repository = scope.ServiceProvider.GetRequiredService<IBookingSeatLockRepository>();
				await repository.CleanupExpiredLocks();
			}, null, TimeSpan.Zero, TimeSpan.FromMinutes(1));

			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Dispose();
			return Task.CompletedTask;
		}
	}
}