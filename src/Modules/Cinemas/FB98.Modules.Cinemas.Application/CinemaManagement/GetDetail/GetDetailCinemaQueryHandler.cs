using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;

namespace FB98.Modules.Cinemas.Application.CinemaManagement.GetDetail
{
	internal sealed class GetDetailCinemaQueryHandler : IQueryHandler<GetDetailCinemaQuery, ApiResult<GetDetailCinemaResponse>>
	{
		private readonly ICinemaRepository _cinemaRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailCinemaQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailCinemaQueryHandler(
			ILogger<GetDetailCinemaQueryHandler> logger,
			ICinemaRepository cinemaRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper)
		{
			_logger = logger;
			_cinemaRepository = cinemaRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<GetDetailCinemaResponse>> Handle(GetDetailCinemaQuery request, CancellationToken cancellationToken)
		{
			var cinemaId = request.CinemaId;
			try
			{
				var cinema = await _cinemaRepository.GetByIdAsync(cinemaId);
				if (cinema == null)
				{
					return ApiResponseBuilder.Error<GetDetailCinemaResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailCinemaResponse>(cinema);
				response.HallsCount = cinema.CinemaHalls.Count();
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail cinema");
				return ApiResponseBuilder.Error<GetDetailCinemaResponse>("An unexpected error occurred", 500);
			}
		}
	}
}