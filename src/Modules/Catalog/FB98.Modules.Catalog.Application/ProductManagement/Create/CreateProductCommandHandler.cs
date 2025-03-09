using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Abstractions.Events;
using FB98.Shared.Infrastructure.Cloudinaries;
using MassTransit;

namespace FB98.Modules.Catalog.Application.ProductManagement.Create
{
	internal sealed class CreateProductCommandHandler : ICommandHandler<CreateProductCommand, ApiResult<object>>
	{
		private readonly IBus _bus;
		private readonly ICategoryRepository _categoryRepository;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateProductCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateProductDto> _validator;

		public CreateProductCommandHandler(
			ILogger<CreateProductCommandHandler> logger,
			IValidator<CreateProductDto> validator,
			IProductRepository productRepository,
			IMapper mapper,
			IUnitOfWork unitOfWork,
			ICloudinaryService cloudinaryService,
			ILocalizedMessageService localizedMessageService,
			IBus bus,
			ICategoryRepository categoryRepository)
		{
			_logger = logger;
			_validator = validator;
			_productRepository = productRepository;
			_mapper = mapper;
			_unitOfWork = unitOfWork;
			_cloudinaryService = cloudinaryService;
			_localizedMessageService = localizedMessageService;
			_bus = bus;
			_categoryRepository = categoryRepository;
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

				if (await _categoryRepository.GetByIdAsync(model.CategoryId) == null)
				{
					return ApiResponseBuilder.Error<object>("Category: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var product = _mapper.Map<Product>(model);

				await _productRepository.CreateAsync(product);
				await _unitOfWork.SaveChangesAsync();
				await _bus.Publish(new ProductCreatedEvent(product.Id, model.StockQuantity!.Value, model.StockIsLimited!.Value), cancellationToken);

				return ApiResponseBuilder.Success<object>(product.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create product");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}