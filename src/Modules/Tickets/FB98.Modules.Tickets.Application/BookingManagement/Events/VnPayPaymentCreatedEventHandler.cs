using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;

namespace FB98.Modules.Tickets.Application.BookingManagement.Events
{
	public class VnPayPaymentCreatedEventHandler : IConsumer<VnPayPaymentCreatedEvent>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly IBookingSeatRepository _bookingSeatRepository;
		private readonly ILogger<VnPayPaymentCreatedEventHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public VnPayPaymentCreatedEventHandler(
			IBookingRepository bookingRepository,
			IBookingSeatLockRepository bookingSeatLockRepository,
			IBookingSeatRepository bookingSeatRepository,
			ILogger<VnPayPaymentCreatedEventHandler> logger,
			IUnitOfWork unitOfWork)
		{
			_bookingRepository = bookingRepository;
			_bookingSeatLockRepository = bookingSeatLockRepository;
			_bookingSeatRepository = bookingSeatRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task Consume(ConsumeContext<VnPayPaymentCreatedEvent> context)
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
					_logger.LogError("Booking not found for ID: {BookingId}", bookingId);
					return;
				}

				if (booking.StatusId != BookingStatusConstants.Created)
				{
					_logger.LogWarning("Booking status is not 'Created' for ID: {BookingId}", bookingId);
					return;
				}

				booking.StatusId = BookingStatusConstants.Pending;

				var bookingSeats = booking.BookingSeats.ToList();
				var seatIds = bookingSeats.Select(seat => seat.SeatId).ToList();
				await _bookingSeatLockRepository.ExtendLockForPayment(context.Message.UserId, booking.ShowId, seatIds);

				await _unitOfWork.SaveChangesAsync();

				_logger.LogInformation("Booking and seats status updated to 'Pending' for ID: {BookingId}", bookingId);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while processing VnPayPaymentCreatedEvent for Booking ID: {BookingId}", context.Message.BookingId);
			}
		}
	}
}