using FB98.Modules.Catalog.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetAllWCategory;

internal sealed class GetAllWCategoryQueryHandler : IQueryHandler<GetAllWCategoryQuery, ApiResult<List<GetAllWCategoryResponse>>>
{
	private readonly IProductRepository _productRepository;
	private readonly ILocalizedMessageService _localizedMessageService;
	private readonly ILogger<GetAllWCategoryQueryHandler> _logger;

	public GetAllWCategoryQueryHandler(
		IProductRepository productRepository,
		ILocalizedMessageService localizedMessageService,
		ILogger<GetAllWCategoryQueryHandler> logger)
	{
		_productRepository = productRepository;
		_localizedMessageService = localizedMessageService;
		_logger = logger;
	}

	public async Task<ApiResult<List<GetAllWCategoryResponse>>> Handle(GetAllWCategoryQuery request, CancellationToken cancellationToken)
	{
		try
		{
			var products = await _productRepository
				.GetAll()
				.Include(p => p.Category)
				.Include(p => p.DiscountRules)
				.ToListAsync(cancellationToken);

			if (!products.Any())
			{
				return ApiResponseBuilder.Error<List<GetAllWCategoryResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
			}

			var result = products
				.GroupBy(p => new { p.CategoryId, p.Category.Name })
				.Select(group => new GetAllWCategoryResponse
				{
					CategoryId = group.Key.CategoryId,
					CategoryName = group.Key.Name,
					Products = group.Select(p => new GetAllWCategoryResponse.ProductResponse
					{
						Id = p.Id,
						Name = p.Name,
						Image = p.Image,
						Price = p.Price,
						DiscountPrice = p.GetDiscountedPrice()
					}).ToList()
				})
				.ToList();

			return ApiResponseBuilder.Success(result, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
		}
		catch (Exception ex)
		{
			_logger.LogError(ex, "Error occurred while retrieving all products by category");
			return ApiResponseBuilder.Error<List<GetAllWCategoryResponse>>("An unexpected error occurred", 500);
		}
	}
}
