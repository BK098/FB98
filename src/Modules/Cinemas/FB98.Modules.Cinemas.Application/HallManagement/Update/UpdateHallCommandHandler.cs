using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;

namespace FB98.Modules.Cinemas.Application.HallManagement.Update
{
	internal sealed class UpdateHallCommandHandler : ICommandHandler<UpdateHallCommand, ApiResult<object>>
	{
		private readonly ICinemaHallRepository _hallRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateHallCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ISeatTypeRepository _seatTypeRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateHallDto> _validator;

		public UpdateHallCommandHandler(
			IMapper mapper,
			IUnitOfWork unitOfWork,
			IValidator<UpdateHallDto> validator,
			ICinemaHallRepository hallRepository,
			ILogger<UpdateHallCommandHandler> logger,
			ILocalizedMessageService localizedMessageService,
			ISeatTypeRepository seatTypeRepository)
		{
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_hallRepository = hallRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
			_seatTypeRepository = seatTypeRepository;
		}

		public async Task<ApiResult<object>> Handle(UpdateHallCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var hallId = request.HallId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var hall = await _hallRepository.GetByIdAsync(hallId);
				if (hall == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (hall.Name != model.Name && await _hallRepository.IsCinemaHallExisted(hall.CinemaId, model.Name!))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				_mapper.Map(model, hall);
				foreach (var updateSeat in model.Seats)
				{
					var seat = hall.Seats.FirstOrDefault(s => s.Id == updateSeat.SeatId);
					if (seat == null)
					{
						_logger.LogWarning($"Seat not found: {updateSeat.SeatId}");
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
					}

					if (seat.SeatTypeId != updateSeat.SeatTypeId)
					{
						var seatType = await _seatTypeRepository.FindByIdAsync(updateSeat.SeatTypeId);
						if (seatType == null)
						{
							_logger.LogWarning($"SeatType not found: {updateSeat.SeatTypeId}");
							return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
						}

						seat.SeatTypeId = updateSeat.SeatTypeId;
					}
				}

				_hallRepository.Update(hall);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update hall");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}