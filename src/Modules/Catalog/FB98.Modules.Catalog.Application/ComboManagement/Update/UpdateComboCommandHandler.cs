using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Cloudinaries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.ComboManagement.Update
{
	internal sealed class UpdateComboCommandHandler : ICommandHandler<UpdateComboCommand, ApiResult<object>>
	{
		private readonly ICloudinaryService _cloudinaryService;
		private readonly IComboRepository _comboRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateComboCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductRepository _productRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateComboDto> _validator;

		public UpdateComboCommandHandler(
			IMapper mapper,
			ILogger<UpdateComboCommandHandler> logger,
			IComboRepository comboRepository,
			IProductRepository productRepository,
			IUnitOfWork unitOfWork,
			IValidator<UpdateComboDto> validator,
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

		public async Task<ApiResult<object>> Handle(UpdateComboCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var comboId = request.ComboId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var combo = await _comboRepository.GetByIdAsync(comboId);
				if (combo is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var productIds = model.Products.Select(p => p.ProductId).ToList();
				var existingProducts = await _productRepository.GetAll()
					.Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken);
				if (existingProducts.Count != productIds.Count)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_mapper.Map(model, combo);
				string? imageUrl;
				if (model.ComboImage != null)
				{
					if (combo.Image != null)
					{
						imageUrl = await _cloudinaryService.ReplaceImageAsync(model.ComboImage!, "catalog/combo", combo.Image);
						combo.Image = imageUrl;
					}
					else
					{
						imageUrl = await _cloudinaryService.UploadImageAsync(model.ComboImage!, "catalog/combo");
						combo.Image = imageUrl;
					}
				}

				await UpdateProducts(combo, model.Products);
				_unitOfWork.Entry(combo, EntityState.Modified);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Updated"), 200);
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Error occurred while updating combo");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating combo");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}

		private async Task UpdateProducts(Combo combo, IList<UpdateComboProductDto> productDtos)
		{
			var existingProductIds = combo.ComboProducts.Select(c => c.ProductId).ToList();
			var newProductIds = productDtos.Select(c => c.ProductId!.Value).ToList();

			var productToRemove = combo.ComboProducts.Where(c => !newProductIds.Contains(c.ProductId)).ToList();
			foreach (var product in productToRemove)
			{
				_unitOfWork.Entry(product, EntityState.Deleted);
			}

			var productMembersToAdd = newProductIds.Except(existingProductIds).ToList();
			var allProductMembers = await _productRepository.GetByIdsAsync(productMembersToAdd);

			foreach (var productId in productMembersToAdd)
			{
				var productMember = allProductMembers.FirstOrDefault(cm => cm.Id == productId);
				if (productMember != null)
				{
					var newMovieProductMember = new ComboProduct
					{
						ProductId = productMember.Id,
						ComboId = combo.Id
					};
					_unitOfWork.Entry(newMovieProductMember, EntityState.Added);
				}
			}
		}
	}
}