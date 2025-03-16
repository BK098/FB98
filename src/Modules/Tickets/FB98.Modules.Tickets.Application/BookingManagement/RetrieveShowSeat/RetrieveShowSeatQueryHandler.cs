using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using Refit;

namespace FB98.Modules.Tickets.Application.BookingManagement.RetrieveShowSeat
{
	internal sealed class RetrieveShowSeatQueryHandler : IQueryHandler<RetrieveShowSeatQuery, ApiResult<RetrieveShowSeatResponse>>
	{
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly IBookingSeatRepository _bookingSeatRepository;
		private readonly ICinemaApi _cinemaApi;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<RetrieveShowSeatQueryHandler> _logger;
		private readonly IShowApi _showApi;

		public RetrieveShowSeatQueryHandler(
			IShowApi showApi,
			ICinemaApi cinemaApi,
			IBookingSeatRepository bookingSeatRepository,
			IBookingSeatLockRepository bookingSeatLockRepository,
			ILogger<RetrieveShowSeatQueryHandler> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_showApi = showApi;
			_cinemaApi = cinemaApi;
			_bookingSeatRepository = bookingSeatRepository;
			_bookingSeatLockRepository = bookingSeatLockRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<RetrieveShowSeatResponse>> Handle(RetrieveShowSeatQuery request, CancellationToken cancellationToken)
		{
			var showId = request.ShowId;
			try
			{
				ApiResult<ShowDto> showResponse;
				try
				{
					showResponse = await _showApi.GetShowById(showId);
					if (showResponse.Data!.ShowStatusId == ShowStatusConstants.Ended)
					{
						return ApiResponseBuilder.Error<RetrieveShowSeatResponse>(_localizedMessageService.GetLocalizedMessage("ShowEnded"), 404);
					}
				}
				catch (ApiException ex)
				{
					_logger.LogWarning(ex.ToString());
					return ApiResponseBuilder.Error<RetrieveShowSeatResponse>("Show: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var show = showResponse.Data;

				ApiResult<CinemaHallDto>? hallResponse;
				try
				{
					hallResponse = await _cinemaApi.GetHallByIdWithSeat(showResponse.Data.CinemaHallId);
				}
				catch (ApiException ex)
				{
					_logger.LogWarning(ex.ToString());
					return ApiResponseBuilder.Error<RetrieveShowSeatResponse>("Hall: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var hallSeats = hallResponse.Data!.Seats.ToDictionary(s => s.SeatId, s => new
				{
					s.SeatTypeId,
					s.SeatPosition
				});

				var bookedSeats = await _bookingSeatRepository.GetBookedSeatsByShow(request.ShowId);
				var bookedSeatIds = bookedSeats.Select(bs => bs.SeatId).ToHashSet();

				var lockedSeats = await _bookingSeatLockRepository.GetLockedSeatsByShow(request.ShowId);
				var lockedSeatIds = lockedSeats.Select(ls => ls.SeatId).ToHashSet();

				var response = new RetrieveShowSeatResponse
				{
					ShowId = request.ShowId,
					MovieTitle = show.MovieTitle,
					StartTime = show.StartTime,
					EndTime = show.EndTime,
					HallId = show.CinemaHallId,
					Seats = hallSeats.Select(seat => new ShowSeatDto
					{
						SeatId = seat.Key,
						SeatPosition = seat.Value.SeatPosition,
						SeatTypeId = seat.Value.SeatTypeId,
						SeatType = SeatTypeConstants.GetStatusName(seat.Value.SeatTypeId),
						SeatStatus =
								bookedSeatIds.Contains(seat.Key) ?
									(bookedSeats.First(bs => bs.SeatId == seat.Key).SeatStatusId == BookingSeatStatusConstants.Pending ? BookingSeatStatusConstants.GetStatusName(BookingSeatStatusConstants.Pending) :
										bookedSeats.First(bs => bs.SeatId == seat.Key).SeatStatusId == BookingSeatStatusConstants.CheckIn ? BookingSeatStatusConstants.GetStatusName(BookingSeatStatusConstants.CheckIn) :
										BookingSeatStatusConstants.GetStatusName(BookingSeatStatusConstants.Booked)) : lockedSeatIds.Contains(seat.Key)
										? _localizedMessageService.GetLocalizedMessage("SeatLocked") :
									_localizedMessageService.GetLocalizedMessage("SeatAvailable")
					}).ToList()
				};

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while retrieve show seat");
				return ApiResponseBuilder.Error<RetrieveShowSeatResponse>("An unexpected error occurred", 500);
			}
		}
	}
}