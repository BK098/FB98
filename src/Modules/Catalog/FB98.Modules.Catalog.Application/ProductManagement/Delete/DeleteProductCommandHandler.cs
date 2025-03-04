using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Infrastructure.Cloudinaries;
using MassTransit;

namespace FB98.Modules.Catalog.Application.ProductManagement.Delete
{
	internal sealed class DeleteProductCommandHandler : ICommandHandler<DeleteProductCommand, ApiResult<object>>
	{
		private readonly IBus _bus;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteProductCommandHandler> _logger;
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;

		public DeleteProductCommandHandler(
			IProductRepository productRepository,
			ILogger<DeleteProductCommandHandler> logger,
			IUnitOfWork unitOfWork,
			ILocalizedMessageService localizedMessageService,
			ICloudinaryService cloudinaryService,
			IBus bus)
		{
			_productRepository = productRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_localizedMessageService = localizedMessageService;
			_cloudinaryService = cloudinaryService;
			_bus = bus;
		}

		public async Task<ApiResult<object>> Handle(DeleteProductCommand request, CancellationToken cancellationToken)
		{
			var productId = request.ProductId;
			try
			{
				var product = await _productRepository.GetByIdAsync(productId);
				if (product is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_cloudinaryService.DeleteImage(product.Image);
				_productRepository.Delete(product);
				await _unitOfWork.SaveChangesAsync();
				await _bus.Publish(new ProductDeletedEvent(productId), cancellationToken);
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Error occurred while deleting combo");
				return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while forgot password");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}