using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetAll
{
	internal sealed class GetAllProductQueryHandler : IQueryHandler<GetAllProductQuery, ApiResult<PaginatedResult<GetAllProductResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name", "Price"];
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllProductQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductRepository _productRepository;

		public GetAllProductQueryHandler(
			ILogger<GetAllProductQueryHandler> logger,
			IProductRepository productRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper)
		{
			_logger = logger;
			_productRepository = productRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllProductResponse>>> Handle(GetAllProductQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var products = _productRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					products = products.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim()
						.Contains(search));
				}

				if (!await products.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllProductResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				products = products.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);
				products = products.Include(x => x.DiscountRules);
				var paginatedResult = await PaginatedResult<Product>.CreateAsync(
					products,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var paginatedProductForView = new PaginatedResult<GetAllProductResponse>(
					_mapper.Map<List<GetAllProductResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(paginatedProductForView, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all products");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllProductResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}