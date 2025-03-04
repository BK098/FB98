using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;

namespace FB98.Modules.Movies.Application.CastManagement.GetDetail
{
	internal sealed class GetDetailCastQueryHandler : IQueryHandler<GetDetailCastQuery, ApiResult<GetDetailCastResponse>>
	{
		private readonly ICastRepository _castRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailCastQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailCastQueryHandler(
			ILogger<GetDetailCastQueryHandler> logger,
			ICastRepository castRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_castRepository = castRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<GetDetailCastResponse>> Handle(GetDetailCastQuery request, CancellationToken cancellationToken)
		{
			var castId = request.CastId;
			try
			{
				var cast = await _castRepository.GetByIdAsync(castId);
				if (cast == null)
				{
					return ApiResponseBuilder.Error<GetDetailCastResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailCastResponse>(cast);
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail cast");
				return ApiResponseBuilder.Error<GetDetailCastResponse>("An unexpected error occurred", 500);
			}
		}
	}
}