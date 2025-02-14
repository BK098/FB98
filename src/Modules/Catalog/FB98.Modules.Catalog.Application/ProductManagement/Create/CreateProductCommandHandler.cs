using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Abstractions.Events.Products;
using FB98.Shared.Abstractions.Modules;
using FB98.Shared.Infrastructure.Cloudinaries;

namespace FB98.Modules.Catalog.Application.ProductManagement.Create
{
	internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ApiResponse<object>>
	{
		private readonly ILogger<CreateProductCommandHandler> _logger;
		private readonly IValidator<CreateProductDto> _validator;
		private readonly IProductRepository _productRepository;
		private readonly IModuleClient _moduleClient;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly ILocalizedMessageService _localizedMessageService;
		public CreateProductCommandHandler(
			ILogger<CreateProductCommandHandler> logger,
			IValidator<CreateProductDto> validator,
			IProductRepository productRepository,
			IModuleClient moduleClient,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			ICloudinaryService cloudinaryService,
			ILocalizedMessageService localizedMessageService)
		{
			_logger = logger;
			_validator = validator;
			_productRepository = productRepository;
			_moduleClient = moduleClient;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_cloudinaryService = cloudinaryService;
			_localizedMessageService = localizedMessageService;
		}
		public async Task<ApiResponse<object>> Handle(CreateProductCommand request, CancellationToken cancellationToken)
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
				await _moduleClient.PublishAsync(new ProductCreatedEvent(product.Id, model.Quantity!.Value));

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
