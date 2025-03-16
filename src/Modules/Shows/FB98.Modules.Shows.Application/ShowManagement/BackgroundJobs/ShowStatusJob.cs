using FB98.Modules.Shows.Application.Abstractions;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Shows.Application.ShowManagement.BackgroundJobs
{
	public sealed class ShowStatusJob : IHostedService, IDisposable
	{
		private readonly ILogger<ShowStatusJob> _logger;
		private readonly IServiceScopeFactory _scopeFactory;
		private Timer? _timer;

		public ShowStatusJob(
			ILogger<ShowStatusJob> logger,
			IServiceScopeFactory scopeFactory)
		{
			_logger = logger;
			_scopeFactory = scopeFactory;
		}

		public void Dispose()
		{
			_timer?.Dispose();
		}

		public Task StartAsync(CancellationToken cancellationToken)
		{
			var taskPeriod = TimeSpan.FromMinutes(5);
			_timer = new Timer(UpdateShowStatuses, null, TimeSpan.Zero, taskPeriod);
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		private async void UpdateShowStatuses(object? state)
		{
			try
			{
				_logger.LogInformation("ShowStatusJob running at {Time}", DateTime.UtcNow);
				using var scope = _scopeFactory.CreateScope();
				var showRepository = scope.ServiceProvider.GetRequiredService<IShowRepository>();
				var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();

				var now = DateTime.UtcNow;

				// Lấy danh sách suất chiếu cần cập nhật trạng thái
				var shows = showRepository.GetAll();

				if (!shows.Any())
				{
					return;
				}

				var upComingShows = shows.Where(x => x.StartTime > now).ToList();
				var onGoingShows = shows.Where(x => x.StartTime <= now && x.EndTime > now).ToList();
				var endedShows = shows.Where(x => x.EndTime <= now).ToList();

				foreach (var show in upComingShows.Where(show => show.ShowStatusId != ShowStatusConstants.UpComming))
				{
					_logger.LogInformation("Updating Show {showId} to UpComing.", show.Id);
					show.ShowStatusId = ShowStatusConstants.UpComming;
				}

				foreach (var show in onGoingShows.Where(show => show.ShowStatusId != ShowStatusConstants.OnGoing))
				{
					_logger.LogInformation("Updating Show {showId} to OnGoing.", show.Id);
					show.ShowStatusId = ShowStatusConstants.OnGoing;
				}

				foreach (var show in endedShows.Where(show => show.ShowStatusId != ShowStatusConstants.Ended))
				{
					_logger.LogInformation("Updating Show {showId} to Ended.", show.Id);
					show.ShowStatusId = ShowStatusConstants.Ended;
				}

				await unitOfWork.SaveChangesAsync();
				_logger.LogInformation("ShowStatusJob completed successfully at {Time}", DateTime.UtcNow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating show statuses.");
			}
		}
	}
}