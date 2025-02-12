using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Infrastructure.Cloudinaries;

namespace FB98.Modules.Catalog.Application.ProductManagement.Update
{
	internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, ApiResponse<object>>
	{
		private readonly IProductRepository _productRepository;
		private readonly ILogger<UpdateProductCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IMapper _mapper;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ICloudinaryService _cloudinaryService;
		public UpdateProductCommandHandler(
			IProductRepository productRepository,
			ILogger<UpdateProductCommandHandler> logger,
			IUnitOfWork unitOfWork,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			ICloudinaryService cloudinaryService)
		{
			_productRepository = productRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_cloudinaryService = cloudinaryService;
		}
		public async Task<ApiResponse<object>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var productId = request.ProductId;
			try
			{
				var product = await _productRepository.GetByIdAsync(productId);
				if (product == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}
				_mapper.Map(model, product);
				if (model.ProductImage is not null)
				{
					string? imageUrl = await _cloudinaryService.ReplaceImageAsync(model.ProductImage!, "catalog/product", product.Image);
					product.Image = imageUrl;
				}
				_productRepository.Update(product);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(product, _localizedMessageService.GetLocalizedMessage("Updated"), statusCode: 200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update product");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
