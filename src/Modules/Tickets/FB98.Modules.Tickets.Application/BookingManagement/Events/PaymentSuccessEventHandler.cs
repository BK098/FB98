using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Email;
using MassTransit;

namespace FB98.Modules.Tickets.Application.BookingManagement.Events
{
	public class PaymentSuccessEventHandler : IConsumer<PaymentSuccessEvent>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IEmailSender _emailSender;
		private readonly ILogger<PaymentSuccessEventHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public PaymentSuccessEventHandler(
			ILogger<PaymentSuccessEventHandler> logger,
			IBookingRepository bookingRepository,
			IEmailSender emailSender,
			IUnitOfWork unitOfWork)
		{
			_logger = logger;
			_bookingRepository = bookingRepository;
			_emailSender = emailSender;
			_unitOfWork = unitOfWork;
		}

		public async Task Consume(ConsumeContext<PaymentSuccessEvent> context)
		{
			try
			{
				var bookingId = context.Message.BookingId;
				if (bookingId == null)
				{
					_logger.LogInformation("BookingId is null, skipping order processing.");
					await context.ConsumeCompleted;
					return;
				}

				var booking = await _bookingRepository.GetByIdAsync(bookingId);
				if (booking == null)
				{
					_logger.LogWarning("Booking not found for ID: {BookingId}", bookingId);
					return;
				}

				if (booking.StatusId != BookingStatusConstants.Pending)
				{
					_logger.LogWarning("Booking status is not 'Pending' for ID: {BookingId}", bookingId);
					return;
				}

				booking.StatusId = BookingStatusConstants.Confirmed;

				var bookingSeats = booking.BookingSeats.ToList();
				foreach (var seat in bookingSeats)
				{
					seat.SeatStatusId = BookingSeatStatusConstants.Booked;
					seat.IsReserved = true;
				}

				await _unitOfWork.SaveChangesAsync();
				_logger.LogInformation("Booking and seats status updated to 'Confirmed' and seat locks removed for ID: {BookingId}", context.Message.BookingId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing PaymentSuccessEvent for Booking ID: {BookingId}", context.Message.BookingId);
			}
		}
	}
}