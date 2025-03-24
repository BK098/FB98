using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.StatusConstants;

namespace FB98.Modules.Tickets.Application.BookingManagement.CheckIn
{
	internal class CheckInCommandHandler : ICommandHandler<CheckInCommand, ApiResult<object>>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CheckInCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;

		public CheckInCommandHandler(
			IBookingRepository bookingRepository,
			ILogger<CheckInCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			IUnitOfWork unitOfWork)
		{
			_bookingRepository = bookingRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(CheckInCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var booking = await _bookingRepository.GetByIdAsync(model.BookingId);
				if (booking == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (booking.StatusId != BookingStatusConstants.Confirmed)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotConfirmedValid"));
				}

				var currentTime = DateTime.UtcNow;
				var showStart = Convert.ToDateTime(booking.ShowStart);
				var showEnd = Convert.ToDateTime(booking.ShowEnd);

				if (currentTime < showStart || currentTime > showEnd)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("ShowNotInProgress"));
				}

				var seatIds = model.SeatIds.ToHashSet();
				var seatsToCheckIn = booking.BookingSeats.Where(x => seatIds.Contains(x.SeatId)).ToList();

				if (!seatsToCheckIn.Any())
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("SeatsNotFound"));
				}

				foreach (var seat in seatsToCheckIn)
				{
					seat.SeatStatusId = BookingSeatStatusConstants.CheckIn;
				}

				_bookingRepository.Update(booking);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(booking.Id);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while check in");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}