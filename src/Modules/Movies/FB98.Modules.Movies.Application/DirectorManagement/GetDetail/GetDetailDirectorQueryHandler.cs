using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;

namespace FB98.Modules.Movies.Application.DirectorManagement.GetDetail
{
	internal sealed class GetDetailDirectorQueryHandler : IQueryHandler<GetDetailDirectorQuery, ApiResult<GetDetailDirectorResponse>>
	{
		private readonly IDirectorRepository _directorRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailDirectorQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailDirectorQueryHandler(IDirectorRepository directorRepository, ILocalizedMessageService localizedMessageService, ILogger<GetDetailDirectorQueryHandler> logger, IMapper mapper)
		{
			_directorRepository = directorRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<GetDetailDirectorResponse>> Handle(GetDetailDirectorQuery request, CancellationToken cancellationToken)
		{
			var directorId = request.DirectorId;
			try
			{
				var director = await _directorRepository.GetByIdAsync(directorId);
				if (director == null)
				{
					return ApiResponseBuilder.Error<GetDetailDirectorResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailDirectorResponse>(director);
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail director");
				return ApiResponseBuilder.Error<GetDetailDirectorResponse>("An unexpected error occurred", 500);
			}
		}
	}
}