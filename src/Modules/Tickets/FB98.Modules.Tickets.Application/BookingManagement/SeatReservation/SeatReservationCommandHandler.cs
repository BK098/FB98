using AutoMapper;
using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using MassTransit;
using Refit;

namespace FB98.Modules.Tickets.Application.BookingManagement.SeatReservation
{
	internal sealed class SeatReservationCommandHandler : ICommandHandler<SeatReservationCommand, ApiResult<object>>
	{
		private readonly IBookingRepository _bookingRepository;
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly IBus _bus;
		private readonly ICinemaApi _cinemaApi;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<SeatReservationCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ISeatPriceRuleRepository _seatPriceRuleRepository;
		private readonly IShowApi _showApi;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<SeatReservationDto> _validator;

		public SeatReservationCommandHandler(
			IBookingRepository bookingRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<SeatReservationCommandHandler> logger,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<SeatReservationDto> validator,
			ICinemaApi cinemaApi,
			IBookingSeatLockRepository bookingSeatRepository,
			ISeatPriceRuleRepository seatPriceRuleRepository,
			IBus bus,
			IShowApi showApi)
		{
			_bookingRepository = bookingRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_cinemaApi = cinemaApi;
			_bookingSeatLockRepository = bookingSeatRepository;
			_seatPriceRuleRepository = seatPriceRuleRepository;
			_bus = bus;
			_showApi = showApi;
		}

		public async Task<ApiResult<object>> Handle(SeatReservationCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
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

				var seats = await _bookingSeatLockRepository.GetLockedSeatsByUser(model.CustomerId!.Value, model.ShowId!.Value);
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
					_logger.LogError(ex.ToString());
					return ApiResponseBuilder.Error<object>("Hall: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var hallSeats = hallResponse.Data!.SeatIds.ToHashSet();

				if (hallSeats.Count != seats.Count)
				{
					return ApiResponseBuilder.Error<object>($"Invalid seat selection. Expected {seats.Count}, but got {hallSeats.Count}");
				}

				var booking = new Booking
				{
					CustomerId = model.CustomerId,
					ShowId = model.ShowId!.Value,
					StatusId = BookingStatusConstants.Created,
					BookingSeats = new List<BookingSeat>()
				};

				decimal totalPrice = 0;
				foreach (var seatId in seatIds)
				{
					var seatTypeId = hallSeats.FirstOrDefault(seat => seat.ContainsKey(seatId))?.GetValueOrDefault(seatId);

					if (seatTypeId == null)
					{
						return ApiResponseBuilder.Error<object>($"Cannot find seat type for seat {seatId}");
					}

					var showDate = Convert.ToDateTime(showResponse.Data!.StartTime).ToUniversalTime();
					var seatPrice = await _seatPriceRuleRepository.GetSeatPriceByTypeAndDate(seatTypeId!.Value, showDate);
					if (seatPrice == null)
					{
						return ApiResponseBuilder.Error<object>($"Cannot determine price for seat {seatId}");
					}

					totalPrice += seatPrice.Price;
					booking.BookingSeats.Add(new BookingSeat
					{
						SeatId = seatId,
						SeatStatusId = BookingSeatStatusConstants.Pending,
						Price = seatPrice.Price,
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
	}
}