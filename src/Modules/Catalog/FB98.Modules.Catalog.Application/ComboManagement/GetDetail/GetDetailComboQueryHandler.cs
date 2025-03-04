using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.ComboManagement.GetDetail
{
	internal sealed class GetDetailComboQueryHandler : IQueryHandler<GetDetailComboQuery, ApiResult<GetDetailComboResponse>>
	{
		private readonly IComboRepository _comboRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailComboQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetDetailComboQueryHandler(
			ILogger<GetDetailComboQueryHandler> logger,
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
					return ApiResponseBuilder.Error<GetDetailComboResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailComboResponse>(combo);
				response.DiscountPrice = combo.GetDiscountedPrice();

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail combo");
				return ApiResponseBuilder.Error<GetDetailComboResponse>("An unexpected error occurred", 500);
			}
		}
	}
}