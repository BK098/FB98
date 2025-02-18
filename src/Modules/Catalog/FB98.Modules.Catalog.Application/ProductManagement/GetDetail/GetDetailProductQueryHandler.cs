using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Abstractions.Refits;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetDetail
{
	internal sealed class GetDetailProductQueryHandler : IQueryHandler<GetDetailProductQuery, ApiResult<GetDetailProductResponse>>
	{
		private readonly ILogger<GetDetailProductQueryHandler> _logger;
		private readonly IProductRepository _productRepository;
		private readonly IMapper _mapper;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IWarehouseApi _warehouseApi;

		public GetDetailProductQueryHandler(ILogger<GetDetailProductQueryHandler> logger,
			IProductRepository productRepository,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			IWarehouseApi warehouseApi)
		{
			_logger = logger;
			_productRepository = productRepository;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_warehouseApi = warehouseApi;
		}

		public async Task<ApiResult<GetDetailProductResponse>> Handle(GetDetailProductQuery request, CancellationToken cancellationToken)
		{
			var productId = request.ProductId;
			try
			{
				var product = await _productRepository.GetByIdAsync(productId);
				if (product is null)
				{
					return ApiResponseBuilder.Error<GetDetailProductResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}
				var response = _mapper.Map<GetDetailProductResponse>(product);

				return ApiResponseBuilder.Success(response, statusCode: 200);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get product detail");
				return ApiResponseBuilder.Error<GetDetailProductResponse>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
