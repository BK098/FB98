using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.SignalRHub;
using Microsoft.AspNetCore.SignalR;
using Refit;

namespace FB98.Modules.Tickets.Application.SeatManagement.LockSeat
{
	internal sealed class LockSeatsCommandHandler : ICommandHandler<LockSeatsCommand, ApiResult<object>>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly IBookingSeatRepository _bookingSeatRepository;
		private readonly ICinemaApi _cinemaApi;
		private readonly IHubContext<SeatHub> _hubContext;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<LockSeatsCommandHandler> _logger;
		private readonly IShowApi _showApi;
		private readonly IValidator<LockSeatsDto> _validator;

		public LockSeatsCommandHandler(
			IBookingSeatLockRepository bookingSeatLockRepository,
			ICinemaApi cinemaApi, ILocalizedMessageService localizedMessageService,
			ILogger<LockSeatsCommandHandler> logger,
			IShowApi showApi,
			IValidator<LockSeatsDto> validator,
			IBookingSeatRepository bookingSeatRepository,
			IHubContext<SeatHub> hubContext,
			IBookingRepository bookingRepository)
		{
			_bookingSeatLockRepository = bookingSeatLockRepository;
			_cinemaApi = cinemaApi;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_showApi = showApi;
			_validator = validator;
			_bookingSeatRepository = bookingSeatRepository;
			_hubContext = hubContext;
			_bookingRepository = bookingRepository;
		}

		public async Task<ApiResult<object>> Handle(LockSeatsCommand request, CancellationToken cancellationToken)
		{
			const int maxSeats = 5;
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var booking = _bookingRepository.GetAll()
					.Where(x => x.UserId == model.UserId! &&
								(x.StatusId == BookingStatusConstants.Created ||
								 x.StatusId == BookingStatusConstants.Pending)).ToList();
				if (booking.Any())
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("PreviousUnpaidBooking"), 404);
				}

				ApiResult<ShowDto>? showResponse;
				try
				{
					showResponse = await _showApi.GetShowById(model.ShowId!.Value);
					if (showResponse.Data!.ShowStatusId == ShowStatusConstants.Ended)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("ShowEnded"), 404);
					}
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<object>("Show: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				ApiResult<CheckSeatsResponse>? hallResponse;
				try
				{
					hallResponse = await _cinemaApi.CheckSeats(showResponse.Data!.CinemaHallId, new CheckSeastsDto(model.SeatIds!.ToList()));
				}
				catch (ApiException ex)
				{
					_logger.LogError(ex.ToString());
					return ApiResponseBuilder.Error<object>("Hall: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var hallSeats = hallResponse.Data!.Seats.ToHashSet();

				if (hallSeats.Count != model.SeatIds!.Count)
				{
					return ApiResponseBuilder.Error<object>($"Invalid seat selection. Expected {model.SeatIds.Count}, but got {hallSeats.Count}");
				}

				var seatLocks = await _bookingSeatLockRepository.GetLockedSeatsByUser(model.UserId!.Value, model.ShowId!.Value);
				if (seatLocks.Count > maxSeats || hallSeats.Count > maxSeats)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("MaxSeatsLimit"));
				}

				var bookedSeats = await _bookingSeatRepository.GetBookedSeatsByShow(model.ShowId!.Value);
				var bookedSeatIds = bookedSeats.Select(bs => bs.SeatId).ToHashSet();

				var availableSeats = model.SeatIds.Where(seatId => !bookedSeatIds.Contains(seatId)).ToList();

				if (availableSeats.Count != model.SeatIds.Count)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("SeatsAlreadyLocked"));
				}

				var unavailableSeats = await _bookingSeatLockRepository.GetLockedSeats(model.ShowId!.Value, model.SeatIds);
				if (unavailableSeats.Any())
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("SeatsAlreadyLocked"));
				}

				await _bookingSeatLockRepository.LockSeats(model.UserId!.Value, model.ShowId!.Value, model.SeatIds!);

				await _hubContext.Clients.All.SendAsync("SeatsStatusChanged", model.ShowId!.Value, cancellationToken);

				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("SeatsLocked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while locking seats");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}