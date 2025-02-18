using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.ComboManagement.GetDetail
{
	internal sealed class GetDetailComboQueryHandler : IQueryHandler<GetDetailComboQuery, ApiResult<GetDetailComboResponse>>
	{
		private readonly ILogger<GetDetailComboQueryHandler> _logger;
		private readonly IComboRepository _comboRepository;
		private readonly IMapper _mapper;
		private readonly ILocalizedMessageService _localizedMessageService;
		public GetDetailComboQueryHandler(ILogger<GetDetailComboQueryHandler> logger,
			IComboRepository comboRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_comboRepository = comboRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
		}
		public async Task<ApiResult<GetDetailComboResponse>> Handle(GetDetailComboQuery request, CancellationToken cancellationToken)
		{
			var comboId = request.ComboId;
			try
			{
				var combo = await _comboRepository.GetByIdAsync(comboId);
				if (combo is null)
				{
					return ApiResponseBuilder.Error<GetDetailComboResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}

				var response = _mapper.Map<GetDetailComboResponse>(combo);

				return ApiResponseBuilder.Success(response, "");
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail combo");
				return ApiResponseBuilder.Error<GetDetailComboResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}