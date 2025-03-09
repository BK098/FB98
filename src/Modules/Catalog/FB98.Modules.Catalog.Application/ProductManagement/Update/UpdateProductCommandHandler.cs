using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Infrastructure.Cloudinaries;

namespace FB98.Modules.Catalog.Application.ProductManagement.Update
{
	internal sealed class UpdateProductCommandHandler : ICommandHandler<UpdateProductCommand, ApiResult<object>>
	{
		private readonly ICategoryRepository _categoryRepository;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateProductCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateProductDto> _validator;

		public UpdateProductCommandHandler(
			IProductRepository productRepository,
			ILogger<UpdateProductCommandHandler> logger,
			IUnitOfWork unitOfWork,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			ICloudinaryService cloudinaryService,
			IValidator<UpdateProductDto> validator,
			ICategoryRepository categoryRepository)
		{
			_productRepository = productRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_cloudinaryService = cloudinaryService;
			_validator = validator;
			_categoryRepository = categoryRepository;
		}

		public async Task<ApiResult<object>> Handle(UpdateProductCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var productId = request.ProductId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				if (await _categoryRepository.GetByIdAsync(model.CategoryId) == null)
				{
					return ApiResponseBuilder.Error<object>("CategoryId: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var product = await _productRepository.GetByIdAsync(productId);
				if (product == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_mapper.Map(model, product);

				_productRepository.Update(product);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(model, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update product");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}