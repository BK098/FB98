using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;

namespace FB98.Modules.Tickets.Application.BookingManagement.Events
{
	public class PaymentSuccessEventHandler : IConsumer<PaymentSuccessEvent>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly ILogger<PaymentSuccessEventHandler> _logger;

		public PaymentSuccessEventHandler(
			ILogger<PaymentSuccessEventHandler> logger,
			IBookingRepository bookingRepository,
			IBookingSeatLockRepository bookingSeatLockRepository)
		{
			_logger = logger;
			_bookingRepository = bookingRepository;
			_bookingSeatLockRepository = bookingSeatLockRepository;
		}

		public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
		{
			try
			{
				var booking = await _bookingRepository.GetByIdAsync(context.Message.BookingId);
				if (booking == null)
				{
					_logger.LogWarning("Booking not found for ID: {BookingId}", context.Message.BookingId);
					return;
				}

				if (booking.StatusId != BookingStatusConstants.Pending)
				{
					_logger.LogWarning("Booking status is not 'Pending' for ID: {BookingId}", context.Message.BookingId);
					return;
				}

				booking.StatusId = BookingStatusConstants.Confirmed;

				var bookingSeats = booking.BookingSeats.ToList();
				foreach (var seat in bookingSeats)
				{
					seat.SeatStatusId = BookingSeatStatusConstants.Booked;
					seat.IsReserved = true;
				}

				await _bookingSeatLockRepository.ReleaseSeatsAfterSuccessfulPayment(context.Message.BookingId!.Value);

				_logger.LogInformation("Booking and seats status updated to 'Confirmed' and seat locks removed for ID: {BookingId}", context.Message.BookingId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing PaymentSuccessEvent for Booking ID: {BookingId}", context.Message.BookingId);
			}
		}
	}
}