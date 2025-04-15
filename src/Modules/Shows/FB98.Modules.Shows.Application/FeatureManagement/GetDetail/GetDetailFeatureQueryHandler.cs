using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.FeatureManagement.GetDetail
{
	public  class GetDetailFeatureQueryHandler : IQueryHandler<GetDetailFeatureQuery, ApiResult<GetDetailFeatureResponse>>
	{
		private readonly IFeatureRepository _featureRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailFeatureQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailFeatureQueryHandler(
			IFeatureRepository featureRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetDetailFeatureQueryHandler> logger,
			IMapper mapper)
		{
			_featureRepository = featureRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<GetDetailFeatureResponse>> Handle(GetDetailFeatureQuery request, CancellationToken cancellationToken)
		{
			var featureId = request.FeatureId;
			try
			{
				var feature = await _featureRepository.GetByIdAsync(featureId);
				if (feature is null)
				{
					return ApiResponseBuilder.Error<GetDetailFeatureResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}

				var response = _mapper.Map<GetDetailFeatureResponse>(feature);
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail feature");
				return ApiResponseBuilder.Error<GetDetailFeatureResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}