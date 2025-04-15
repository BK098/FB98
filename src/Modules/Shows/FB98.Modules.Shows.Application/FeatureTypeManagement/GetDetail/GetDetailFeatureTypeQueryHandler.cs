using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;

namespace FB98.Modules.Shows.Application.FeatureTypeManagement.GetDetail
{
	public  class GetDetailFeatureTypeQueryHandler : IQueryHandler<GetDetailFeatureTypeQuery, ApiResult<GetDetailFeatureTypeResponse>>
	{
		private readonly IFeatureTypeRepository _featureTypeRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailFeatureTypeQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailFeatureTypeQueryHandler(
			IFeatureTypeRepository featureTypeRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetDetailFeatureTypeQueryHandler> logger,
			IMapper mapper)
		{
			_featureTypeRepository = featureTypeRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<GetDetailFeatureTypeResponse>> Handle(GetDetailFeatureTypeQuery request, CancellationToken cancellationToken)
		{
			var featureTypeId = request.FeatureTypeId;
			try
			{
				var featureType = await _featureTypeRepository.GetByIdAsync(featureTypeId);
				if (featureType is null)
				{
					return ApiResponseBuilder.Error<GetDetailFeatureTypeResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailFeatureTypeResponse>(featureType);
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail feature type");
				return ApiResponseBuilder.Error<GetDetailFeatureTypeResponse>("An unexpected error occurred", 500);
			}
		}
	}
}