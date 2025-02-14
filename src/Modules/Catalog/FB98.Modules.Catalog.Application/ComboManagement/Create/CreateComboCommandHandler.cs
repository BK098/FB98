using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Cloudinaries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.ComboManagement.Create
{
	internal sealed class CreateComboCommandHandler : ICommandHandler<CreateComboCommand, ApiResponse<object>>
	{
		private readonly IMapper _mapper;
		private readonly ILogger<CreateComboCommandHandler> _logger;
		private readonly IComboRepository _comboRepository;
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateComboDto> _validator;
		private readonly ICloudinaryService _cloudinaryService;
		private readonly ILocalizedMessageService _localizedMessageService;

		public CreateComboCommandHandler(
			IMapper mapper,
			ILogger<CreateComboCommandHandler> logger,
			IComboRepository comboRepository,
			IProductRepository productRepository,
			IUnitOfWork unitOfWork,
			IValidator<CreateComboDto> validator,
			ICloudinaryService cloudinaryService,
			ILocalizedMessageService localizedMessageService)
		{
			_mapper = mapper;
			_logger = logger;
			_comboRepository = comboRepository;
			_productRepository = productRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_cloudinaryService = cloudinaryService;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResponse<object>> Handle(CreateComboCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			string? imageUrl = null;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var productIds = model.Products.Select(p => p.ProductId).ToList();
				var existingProducts = await _productRepository.GetAll()
					.Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken);

				if (existingProducts.Count != productIds.Count)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), statusCode: 404);
				}

				var combo = _mapper.Map<Combo>(model);
				if (model.ComboImage is not null)
				{
					imageUrl = await _cloudinaryService.UploadImageAsync(model.ComboImage!, "catalog/combo");
				}
				combo.Image = imageUrl;
				await _comboRepository.CreateAsync(combo);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(combo.Id, _localizedMessageService.GetLocalizedMessage("Created"), statusCode: 201);
			}
			catch (Exception ex)
			{
				if (model.ComboImage is not null)
				{
					_cloudinaryService.DeleteImage(imageUrl);
				}

				_logger.LogError(ex, "Error occurred while get create combo");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}
