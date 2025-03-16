using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.StatusConstants;

namespace FB98.Modules.Tickets.Application.BookingManagement.CheckIn
{
	internal class CheckInCommandHandler : ICommandHandler<CheckInCommand, ApiResult<object>>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly ILogger<CheckInCommandHandler> _logger;
		private readonly ILocalizedMessageService _localizedMessageService;
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

		public async Task<ApiResult<object>> Handle(CheckInCommand command, CancellationToken cancellationToken)
		{
			try
			{
				var booking = await _bookingRepository.GetByIdAsync(command.BookingId);
				if (booking == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (booking.StatusId != BookingStatusConstants.Confirmed)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotConfirmedValid"));
				}

				booking.StatusId = BookingStatusConstants.CheckIn;
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