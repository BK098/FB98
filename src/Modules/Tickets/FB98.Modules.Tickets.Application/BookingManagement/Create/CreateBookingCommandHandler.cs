using AutoMapper;
using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Abstractions.Refits;
using Refit;

namespace FB98.Modules.Tickets.Application.BookingManagement.Create
{
	internal sealed class CreateBookingCommandHandler : ICommandHandler<CreateBookingCommand, ApiResult<object>>
	{
		private readonly IBookingRepository _bookingRepository;

		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;

		//private readonly IBus _bus;
		private readonly ICinemaApi _cinemaApi;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateBookingCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IShowApi _showApi;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateBookingDto> _validator;

		public CreateBookingCommandHandler(
			IBookingRepository bookingRepository,
			//IBus bus,
			ILocalizedMessageService localizedMessageService,
			ILogger<CreateBookingCommandHandler> logger,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<CreateBookingDto> validator,
			IShowApi showApi,
			ICinemaApi cinemaApi,
			IBookingSeatLockRepository bookingSeatRepository)
		{
			_bookingRepository = bookingRepository;
			//_bus = bus;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_showApi = showApi;
			_cinemaApi = cinemaApi;
			_bookingSeatLockRepository = bookingSeatRepository;
		}

		public async Task<ApiResult<object>> Handle(CreateBookingCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				// check show
				ApiResult<ShowDto>? showResponse;
				try
				{
					showResponse = await _showApi.GetShowById(model.ShowId);
					var showStatusEnded = Guid.Parse("4cfd3bd1-062d-442f-ad42-fb4726f061e8");
					if (showResponse.Data!.ShowStatusId == showStatusEnded)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("ShowEnded"), 404);
					}
				}
				catch (ApiException)
				{
					return ApiResponseBuilder.Error<object>("Show: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				// check hall
				ApiResult<CheckSeatsResponse>? hallResponse;
				try
				{
					hallResponse = await _cinemaApi.CheckSeats(showResponse.Data!.CinemaHallId, new CheckSeastsDto(model.SeatIds));
				}
				catch (ApiException ex)
				{
					_logger.LogError(ex.ToString());
					return ApiResponseBuilder.Error<object>("Hall: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var hallSeats = hallResponse.Data!.SeatIds.ToHashSet();

				if (hallSeats.Count != model.SeatIds.Count)
				{
					return ApiResponseBuilder.Error<object>($"Invalid seat selection. Expected {model.SeatIds.Count}, but got {hallSeats.Count}");
				}

				// check seat is locked?
				var unavailableSeats = await _bookingSeatLockRepository.GetLockedSeats(model.ShowId, model.SeatIds);
				if (unavailableSeats.Any())
				{
					return ApiResponseBuilder.Error<object>("Some seats are not available");
				}

				var booking = _mapper.Map<Booking>(model);
				foreach (var seat in hallSeats)
				{

				}
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