using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;

namespace FB98.Modules.Cinemas.Application.HallManagement.CheckSeats
{
	public sealed class CheckSeatsCommandHandler : ICommandHandler<CheckSeatsCommand, ApiResult<CheckSeatsResponse>>
	{
		private readonly ICinemaHallRepository _hallRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CheckSeatsCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IValidator<CheckSeatsDto> _validator;

		public CheckSeatsCommandHandler(
			ICinemaHallRepository hallRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<CheckSeatsCommandHandler> logger,
			IMapper mapper,
			IValidator<CheckSeatsDto> validator)
		{
			_hallRepository = hallRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_validator = validator;
		}

		public async Task<ApiResult<CheckSeatsResponse>> Handle(CheckSeatsCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var hallId = request.HallId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<CheckSeatsResponse>(validationResult.Errors);
				}

				var hall = await _hallRepository.GetValidHallSeats(hallId, model.SeatIds);
				if (hall == null)
				{
					return ApiResponseBuilder.Error<CheckSeatsResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var validSeatIds = hall.Seats.Select(s => s.Id).ToList();
				if (!validSeatIds.Any())
				{
					return ApiResponseBuilder.Error<CheckSeatsResponse>(_localizedMessageService.GetLocalizedMessage("NoValidSeats"));
				}

				var response = _mapper.Map<CheckSeatsResponse>(hall);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("Data"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create new discount rule");
				return ApiResponseBuilder.Error<CheckSeatsResponse>("An unexpected error occurred", 500);
			}
		}
	}
}