using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.CategoryManagement.GetDetail
{
	internal sealed class GetDetailCategoryQueryCommand : IQueryHandler<GetDetailCategoryQuery, ApiResult<GetDetailCategoryResponse>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly IMapper _mapper;
		private readonly ILogger<GetDetailCategoryQueryCommand> _logger;
		private readonly ILocalizedMessageService _localizedMessageService;

		public GetDetailCategoryQueryCommand(
			ICategoryRepository categoryRepository,
			IMapper mapper,
			ILogger<GetDetailCategoryQueryCommand> logger,
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
				categoryDto.ProductCount = category.Products.Count;
				return ApiResponseBuilder.Success(categoryDto);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail category");
				return ApiResponseBuilder.Error<GetDetailCategoryResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
