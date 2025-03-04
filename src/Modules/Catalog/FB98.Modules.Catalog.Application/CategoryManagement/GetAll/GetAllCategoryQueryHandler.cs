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
		private readonly ICategoryRepository _categoryRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllCategoryQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllCategoryQueryHandler(
			ILogger<GetAllCategoryQueryHandler> logger,
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
				var entities = _categoryRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim().Contains(search));
				}

				if (!await entities.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllCategoryResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<Category>.CreateAsync(
					entities,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllCategoryResponse>(
					_mapper.Map<List<GetAllCategoryResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all category");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllCategoryResponse>>("An unexpected error occurred",
					500);
			}
		}
	}
}