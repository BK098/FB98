using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.ShowManagement.GetDetail
{
	internal sealed class GetDetailShowQueryHandler : IQueryHandler<GetDetailShowQuery, ApiResult<GetDetailShowResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailShowQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IShowRepository _showRepository;

		public GetDetailShowQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetDetailShowQueryHandler> logger,
			IMapper mapper,
			IShowRepository showRepository)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_showRepository = showRepository;
		}

		public async Task<ApiResult<GetDetailShowResponse>> Handle(GetDetailShowQuery request, CancellationToken cancellationToken)
		{
			var showId = request.ShowId;
			try
			{
				var show = await _showRepository.GetByIdAsync(showId);
				if (show == null)
				{
					return ApiResponseBuilder.Error<GetDetailShowResponse>("Show: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var responnse = _mapper.Map<GetDetailShowResponse>(show);
				return ApiResponseBuilder.Success(responnse, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create new discount rule");
				return ApiResponseBuilder.Error<GetDetailShowResponse>("An unexpected error occurred", 500);
			}
		}
	}
}