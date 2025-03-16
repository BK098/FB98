using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.StatusConstants;
using Refit;

namespace FB98.Modules.Tickets.Application.SeatManagement.LockSeat
{
	internal sealed class LockSeatsCommandHandler : ICommandHandler<LockSeatsCommand, ApiResult<object>>
	{
		private readonly IBookingSeatLockRepository _bookingSeatLockRepository;
		private readonly ICinemaApi _cinemaApi;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<LockSeatsCommandHandler> _logger;
		private readonly IShowApi _showApi;
		private readonly IValidator<LockSeatsDto> _validator;


		public LockSeatsCommandHandler(
			IBookingSeatLockRepository bookingSeatLockRepository,
			ICinemaApi cinemaApi, ILocalizedMessageService localizedMessageService,
			ILogger<LockSeatsCommandHandler> logger,
			IShowApi showApi,
			IValidator<LockSeatsDto> validator)
		{
			_bookingSeatLockRepository = bookingSeatLockRepository;
			_cinemaApi = cinemaApi;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_showApi = showApi;
			_validator = validator;
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

				var hallSeats = hallResponse.Data!.SeatIds.ToHashSet();

				if (hallSeats.Count != model.SeatIds!.Count)
				{
					return ApiResponseBuilder.Error<object>($"Invalid seat selection. Expected {model.SeatIds.Count}, but got {hallSeats.Count}");
				}

				if (hallSeats.Count > maxSeats)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("MaxSeatsLimit"));
				}

				var unavailableSeats = await _bookingSeatLockRepository.GetLockedSeats(model.ShowId!.Value, model.SeatIds);
				if (unavailableSeats.Any())
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("SeatsAlreadyLocked"));
				}

				await _bookingSeatLockRepository.LockSeats(model.CustomerId!.Value, model.ShowId!.Value, model.SeatIds!);

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