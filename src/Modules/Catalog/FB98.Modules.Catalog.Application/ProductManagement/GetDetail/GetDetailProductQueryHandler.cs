using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetDetail
{
	internal sealed class GetDetailProductQueryHandler : IQueryHandler<GetDetailProductQuery, ApiResult<GetDetailProductResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailProductQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductRepository _productRepository;

		public GetDetailProductQueryHandler(
			ILogger<GetDetailProductQueryHandler> logger,
			IProductRepository productRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_productRepository = productRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<GetDetailProductResponse>> Handle(
			GetDetailProductQuery request,
			CancellationToken cancellationToken)
		{
			var productId = request.ProductId;
			try
			{
				var product = await _productRepository.GetByIdAsync(productId);
				if (product is null)
				{
					return ApiResponseBuilder.Error<GetDetailProductResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailProductResponse>(product);
				response.DiscountPrice = product.GetDiscountedPrice();
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get product detail");
				return ApiResponseBuilder.Error<GetDetailProductResponse>("An unexpected error occurred", 500);
			}
		}
	}
}