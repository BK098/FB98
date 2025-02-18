using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Infrastructure.Cloudinaries;
using MassTransit;

namespace FB98.Modules.Catalog.Application.ProductManagement.Create
{
	internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ApiResult<object>>
	{
		private readonly ILogger<CreateProductCommandHandler> _logger;
		private readonly IValidator<CreateProductDto> _validator;
		private readonly IProductRepository _productRepository;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IBus _bus;
		public CreateProductCommandHandler(
			ILogger<CreateProductCommandHandler> logger,
			IValidator<CreateProductDto> validator,
			IProductRepository productRepository,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			ICloudinaryService cloudinaryService,
			ILocalizedMessageService localizedMessageService,
			IBus bus)
		{
			_logger = logger;
			_validator = validator;
			_productRepository = productRepository;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_cloudinaryService = cloudinaryService;
			_localizedMessageService = localizedMessageService;
			_bus = bus;
		}
		public async Task<ApiResult<object>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}
				var product = _mapper.Map<Product>(model);
				if (model.ProductImage is not null)
				{
					string? imageUrl = await _cloudinaryService.UploadImageAsync(model.ProductImage!, "catalog/product");
					product.Image = imageUrl;
				}
				await _productRepository.CreateAsync(product);
				await _unitOfWork.SaveChangesAsync();
				await _bus.Publish(new ProductCreatedEvent(product.Id, model.StockQuantity!.Value, model.StockIsLimited!.Value), cancellationToken);

				return ApiResponseBuilder.Success<object>(product, _localizedMessageService.GetLocalizedMessage("Created"), statusCode: 201);

			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create product");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
