using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.CategoryManagement.GetAll
{
	internal sealed class GetAllCategoryQueryHandler : IQueryHandler<GetAllCategoryQuery, ApiResult<PaginatedResult<GetAllCategoryResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name"];
		private readonly ILogger<GetAllCategoryResponse> _logger;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IMapper _mapper;

		public GetAllCategoryQueryHandler(
			ILogger<GetAllCategoryResponse> logger,
			ICategoryRepository categoryRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper)
		{
			_logger = logger;
			_categoryRepository = categoryRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllCategoryResponse>>> Handle(GetAllCategoryQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var categories = _categoryRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					categories = categories.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim()
						.Contains(search));
				}

				categories = categories.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);
				if (!await categories.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllCategoryResponse>>(
						_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}

				var paginatedResult = await PaginatedResult<Category>.CreateAsync(
					categories,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);
				var categoryForView = _mapper.Map<List<GetAllCategoryResponse>>(paginatedResult.Items);

				var paginatedCategoryForView = new PaginatedResult<GetAllCategoryResponse>(
					categoryForView,
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(paginatedCategoryForView);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all categories");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllCategoryResponse>>("An unexpected error occurred",
					statusCode: 500);
			}
		}
	}
}