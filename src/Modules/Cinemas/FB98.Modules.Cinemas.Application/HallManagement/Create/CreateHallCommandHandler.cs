using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;

namespace FB98.Modules.Cinemas.Application.HallManagement.Create
{
	internal sealed class CreateHallCommandHandler : ICommandHandler<CreateHallCommand, ApiResult<object>>
	{
		private readonly ICinemaHallRepository _cinemaHallRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateHallCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateHallDto> _validator;

		public CreateHallCommandHandler(
			IValidator<CreateHallDto> validator,
			ILogger<CreateHallCommandHandler> logger,
			IMapper mapper,
			ICinemaHallRepository cinemaHallRepository,
			ILocalizedMessageService localizedMessageService,
			IUnitOfWork unitOfWork)
		{
			_validator = validator;
			_logger = logger;
			_mapper = mapper;
			_cinemaHallRepository = cinemaHallRepository;
			_localizedMessageService = localizedMessageService;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(CreateHallCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				if (await _cinemaHallRepository.IsCinemaHallExisted(model.CinemaId!.Value, model.Name!))
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
				}

				var cinemaHall = _mapper.Map<CinemaHall>(model);
				await _cinemaHallRepository.CreateAsync(cinemaHall);
				await CreateSeats(cinemaHall.Id, model.RangeSeatRow!.Value, model.RangeSeatColumn!.Value);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(cinemaHall.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create cinema hall");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}

		public async Task CreateSeats(Guid hallId, int rows, int columns)
		{
			const int chunkSize = 50;
			var seatsChunk = new List<CinemaHallSeat>();

			for (byte row = 1; row <= rows; row++)
			{
				for (byte column = 1; column <= columns; column++)
				{
					var seat = new CinemaHallSeat
					{
						HallId = hallId,
						SeatRow = row,
						SeatColumn = column,
						SeatTypeId = SeatTypeConstants.Normal
					};
					seat.SetSeatPosition(row, column);
					seatsChunk.Add(seat);

					if (seatsChunk.Count < chunkSize)
					{
						continue;
					}

					await _cinemaHallRepository.AddRangeSeatsAsync(seatsChunk);
					await _unitOfWork.SaveChangesAsync();
					seatsChunk.Clear();  // Dọn sạch danh sách ghế
				}
			}
			if (seatsChunk.Count > 0)
			{
				await _cinemaHallRepository.AddRangeSeatsAsync(seatsChunk);
				await _unitOfWork.SaveChangesAsync();
			}
		}
	}
}