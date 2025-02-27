using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.Application.HallManagement.Create;

namespace FB98.Modules.Cinemas.Application.HallManagement.GetDetail
{
	internal sealed class GetDetailHallQueryHandler : IQueryHandler<GetDetailHallQuery, ApiResult<GetDetailHallResponse>>
	{
		private readonly ICinemaHallRepository _cinemaHallRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateHallCommandHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailHallQueryHandler(
			ICinemaHallRepository cinemaHallRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<CreateHallCommandHandler> logger,
			IMapper mapper)
		{
			_cinemaHallRepository = cinemaHallRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<GetDetailHallResponse>> Handle(GetDetailHallQuery request, CancellationToken cancellationToken)
		{
			var hallId = request.HallId;
			try
			{
				var hall = await _cinemaHallRepository.GetByIdAsync(hallId);
				if (hall == null)
				{
					return ApiResponseBuilder.Error<GetDetailHallResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailHallResponse>(hall);
				//response.SeatColumn = hall.Seats.Where
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail hall");
				return ApiResponseBuilder.Error<GetDetailHallResponse>("An unexpected error occurred", 500);
			}
		}
	}
}