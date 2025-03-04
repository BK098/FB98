using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.CategoryManagement.GetDetail
{
	internal sealed class GetDetailCategoryQueryHandler : IQueryHandler<GetDetailCategoryQuery, ApiResult<GetDetailCategoryResponse>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<GetDetailCategoryQueryHandler> _logger;
		private readonly ILocalizedMessageService _localizedMessageService;

		public GetDetailCategoryQueryHandler(
			ICategoryRepository categoryRepository,
			IMapper mapper,
			ILogger<GetDetailCategoryQueryHandler> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_categoryRepository = categoryRepository;
			_mapper = mapper;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<GetDetailCategoryResponse>> Handle(GetDetailCategoryQuery request, CancellationToken cancellationToken)
		{
			var categoryId = request.CategoryId;
			try
			{
				var category = await _categoryRepository.GetByIdAsync(categoryId);
				if (category is null)
				{
					return ApiResponseBuilder.Error<GetDetailCategoryResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}

				var categoryDto = _mapper.Map<GetDetailCategoryResponse>(category);
				categoryDto.ProductCount = category.GetTotalProducts();
				return ApiResponseBuilder.Success(categoryDto, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail category");
				return ApiResponseBuilder.Error<GetDetailCategoryResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}