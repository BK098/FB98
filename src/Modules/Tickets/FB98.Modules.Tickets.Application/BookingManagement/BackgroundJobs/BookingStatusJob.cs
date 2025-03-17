using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.StatusConstants;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;

namespace FB98.Modules.Tickets.Application.BookingManagement.BackgroundJobs
{
	public sealed class BookingStatusJob : IHostedService, IDisposable
	{
		private readonly ILogger<BookingStatusJob> _logger;
		private readonly IServiceScopeFactory _scopeFactory;
		private Timer? _timer;

		public BookingStatusJob(
			ILogger<BookingStatusJob> logger,
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
			var taskPeriod = TimeSpan.FromMinutes(1);
			_timer = new Timer(CheckBookingStatus, null, TimeSpan.Zero, taskPeriod);
			return Task.CompletedTask;
		}

		public Task StopAsync(CancellationToken cancellationToken)
		{
			_timer?.Change(Timeout.Infinite, 0);
			return Task.CompletedTask;
		}

		private async void CheckBookingStatus(object? state)
		{
			try
			{
				_logger.LogInformation("BookingStatusJob running at {Time}", DateTime.UtcNow);
				using var scope = _scopeFactory.CreateScope();
				var bookingRepository = scope.ServiceProvider.GetRequiredService<IBookingRepository>();
				var unitOfWork = scope.ServiceProvider.GetRequiredService<IUnitOfWork>();
				var now = DateTime.UtcNow;

				// Expire bookings that are still in Created state after 5 minutes
				var createdBookings = await bookingRepository.GetBookingsByStatusAndTimeAsync(BookingStatusConstants.Created, now.AddMinutes(-5));
				foreach (var booking in createdBookings)
				{
					_logger.LogInformation("Expiring booking {BookingId}", booking.Id);
					//var bookingStatusHistory = new BookingStatusHistory
					//{
					//    BookingId = booking.Id,
					//    OldStatusId = booking.BookingStatusId,
					//    NewStatusId = BookingStatusConstants.Expired
					//};
					//bookingStatusHistory.SetCreatedAt();
					unitOfWork.Entry(booking, EntityState.Deleted);
					//booking.StatusId = BookingStatusConstants.Expired;
				}

				// Expire bookings that are still in Pending state after 15 minutes
				var pendingBookings = await bookingRepository.GetBookingsByStatusAndTimeAsync(BookingStatusConstants.Pending, now.AddMinutes(-15));
				foreach (var booking in pendingBookings)
				{
					_logger.LogInformation("Expiring pending booking {BookingId}", booking.Id);
					//var bookingStatusHistory = new BookingStatusHistory
					//{
					//	BookingId = booking.Id,
					//	OldStatusId = booking.BookingStatusId,
					//	NewStatusId = BookingStatusConstants.Expired
					//};
					//bookingStatusHistory.SetCreatedAt();
					unitOfWork.Entry(booking, EntityState.Deleted);
					//booking.StatusId = BookingStatusConstants.Expired;
					var bookingSeats = booking.BookingSeats.Where(x => x.BookingId == booking.Id).ToList();
					foreach (var bookingSeat in bookingSeats)
					{
						_logger.LogInformation("Expiring pending booking {BookingId}", bookingSeat.Id);
						//var bookingStatusHistory = new BookingStatusHistory
						//{
						//	BookingId = booking.Id,
						//	OldStatusId = booking.BookingStatusId,
						//	NewStatusId = BookingStatusConstants.Expired
						//};
						//bookingStatusHistory.SetCreatedAt();
						unitOfWork.Entry(bookingSeat, EntityState.Deleted);
					}
				}

				await unitOfWork.SaveChangesAsync();
				_logger.LogInformation("BookingStatusJob completed successfully at {Time}", DateTime.UtcNow);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex.ToString());
			}
		}
	}
}