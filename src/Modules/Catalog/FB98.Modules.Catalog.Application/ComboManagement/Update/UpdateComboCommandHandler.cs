using FB98.Modules.Catalog.Application.Abstractions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.ComboManagement.Update
{
	internal sealed class UpdateComboCommandHandler : ICommandHandler<UpdateComboCommand, ApiResult<object>>
	{
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
			ILocalizedMessageService localizedMessageService)
		{
			_mapper = mapper;
			_logger = logger;
			_comboRepository = comboRepository;
			_productRepository = productRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
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

				var combo = await _comboRepository.GetByIdNoTrackingAsync(comboId);
				if (combo is null)
				{
					return ApiResponseBuilder.Error<object>("Combo: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var productIds = model.Products!.Select(p => p.ProductId).ToList();
				var existingProducts = await _productRepository.GetAll()
					.Where(p => productIds.Contains(p.Id)).ToListAsync(cancellationToken);
				if (existingProducts.Count != productIds.Count)
				{
					return ApiResponseBuilder.Error<object>("Product: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_mapper.Map(model, combo);
				_comboRepository.Update(combo);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(comboId, _localizedMessageService.GetLocalizedMessage("Updated"));
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
	}
}