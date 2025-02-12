using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Abstractions.Events.Base;
using FB98.Shared.Abstractions.Events.Products;
using FB98.Shared.Infrastructure.Cloudinaries;

namespace FB98.Modules.Catalog.Application.ProductManagement.Delete
{
	internal sealed class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, ApiResponse<object>>
	{
		private readonly IProductRepository _productRepository;
		private readonly ILogger<DeleteProductCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IEventDispatcher _eventDispatcher;
		private readonly ICloudinaryService _cloudinaryService;


		public DeleteProductCommandHandler(
			IProductRepository productRepository,
			ILogger<DeleteProductCommandHandler> logger,
			IUnitOfWork unitOfWork,
			ILocalizedMessageService localizedMessageService,
			ICloudinaryService cloudinaryService,
			IEventDispatcher eventDispatcher)
		{
			_productRepository = productRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_localizedMessageService = localizedMessageService;
			_cloudinaryService = cloudinaryService;
			_eventDispatcher = eventDispatcher;
		}

		public async Task<ApiResponse<object>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			var productId = request.ProductId;
			try
			{
				var product = await _productRepository.GetByIdAsync(productId);
				if (product is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}
				_productRepository.Delete(product);
				_cloudinaryService.DeleteImage(product.Image);
				await _unitOfWork.SaveChangesAsync();
				await _eventDispatcher.PublishAsync(new ProductDeletedEvent(productId));
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"), statusCode: 204);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while forgot password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
