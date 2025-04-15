using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using Refit;

namespace FB98.Modules.Tickets.Application.BookingManagement.SeatReservation
{
	public  sealed class SeatReservationCommandHandler : ICommandHandler<SeatReservationCommand, ApiResult<object>>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly ICinemaApi _cinemaApi;
		private readonly ICustomerApi _customerApi;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<SeatReservationCommandHandler> _logger;
		private readonly ISeatPriceRuleRepository _seatPriceRuleRepository;
		private readonly IShowApi _showApi;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<SeatReservationDto> _validator;

		public SeatReservationCommandHandler(
			IBookingRepository bookingRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<SeatReservationCommandHandler> logger,
			IUnitOfWork unitOfWork,
			IValidator<SeatReservationDto> validator,
			ICinemaApi cinemaApi,
			IBookingSeatLockRepository bookingSeatRepository,
			ISeatPriceRuleRepository seatPriceRuleRepository,
			IShowApi showApi,
			ICustomerApi customerApi)
		{
			_bookingRepository = bookingRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_cinemaApi = cinemaApi;
			_bookingSeatLockRepository = bookingSeatRepository;
			_seatPriceRuleRepository = seatPriceRuleRepository;
			_showApi = showApi;
			_customerApi = customerApi;
		}

		public async Task<ApiResult<object>> Handle(SeatReservationCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var bookingDicount = 0;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
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

				var seats = await _bookingSeatLockRepository.GetLockedSeatsByUser(model.UserId!.Value, model.ShowId!.Value);
				if (!seats.Any())
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NoSeatsForReservation"));
				}

				var seatIds = seats.Select(x => x.SeatId).ToList();

				ApiResult<CheckSeatsResponse>? hallResponse;
				try
				{
					hallResponse = await _cinemaApi.CheckSeats(showResponse.Data.CinemaHallId, new CheckSeastsDto(seatIds));
				}
				catch (ApiException ex)
				{
					_logger.LogWarning(ex.ToString());
					return ApiResponseBuilder.Error<object>("Hall: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				try
				{
					var customerResponse = await _customerApi.GetCustomerById(model.UserId!.Value);
					if (!customerResponse.IsSuccess)
					{
						bookingDicount = customerResponse.Data!.MembershipDiscount;
					}
				}
				catch (ApiException ex)
				{
					_logger.LogWarning(ex.ToString());
					//return ApiResponseBuilder.Error<object>("User: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var hallSeats = hallResponse.Data!.Seats.ToDictionary(s => s.SeatId, s => new
				{
					s.SeatTypeId,
					s.SeatPosition
				});

				if (hallSeats.Count != seats.Count)
				{
					return ApiResponseBuilder.Error<object>($"Invalid seat selection. Expected {seats.Count}, but got {hallSeats.Count}");
				}

				var booking = new Booking
				{
					HallId = showResponse.Data.CinemaHallId,
					HallName = hallResponse.Data.Name,
					MovieTitle = showResponse.Data.MovieTitle,
					ShowStart = showResponse.Data.StartTime,
					ShowEnd = showResponse.Data.EndTime,
					UserId = model.UserId!.Value,
					ShowId = model.ShowId!.Value,
					UserName = model.UserName!,
					UserPhone = model.UserPhone!,
					StatusId = BookingStatusConstants.Created,
					BookingSeats = new List<BookingSeat>()
				};

				decimal totalPrice = 0;
				foreach (var seatId in seatIds)
				{
					var seat = hallSeats[seatId];

					var startTime = showResponse.Data.StartTime;
					const string format = "dd-MM-yyyy HH:mm:ss zz";
					var showDate = DateTime.ParseExact(startTime, format, null).ToUniversalTime();
					var seatPrice = await _seatPriceRuleRepository.GetSeatPriceByTypeAndDate(seat.SeatTypeId, showDate);
					if (seatPrice == null)
					{
						return ApiResponseBuilder.Error<object>($"Cannot determine price for seat {seatId}");
					}

					totalPrice += seatPrice.Price;
					booking.BookingSeats.Add(new BookingSeat
					{
						SeatPosition = seat.SeatPosition,
						SeatId = seatId,
						SeatTypeName = SeatTypeConstants.GetStatusName(seat.SeatTypeId),
						SeatStatusId = BookingSeatStatusConstants.Pending,
						Price = BookingDiscount(seatPrice.Price, bookingDicount),
						SeatPriceApplication = new SeatPriceApplication
						{
							SeatPriceRuleId = seatPrice.Id,
							AppliedPrice = seatPrice.Price
						}
					});
				}

				booking.Amount = totalPrice;

				await _bookingRepository.CreateAsync(booking);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(booking.Id, _localizedMessageService.GetLocalizedMessage("Created"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while creating booking");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}

		private static decimal BookingDiscount(decimal amount, int discount)
		{
			var result = amount - amount * discount / 100;
			var roundedUp = Math.Ceiling(result);
			return Math.Floor(roundedUp / 100) * 100;
		}
	}
}