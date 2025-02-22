using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;

namespace FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule
{
	internal class CreateDiscountRuleCommandHandler : ICommandHandler<CreateDiscountRuleCommand, ApiResult<object>>
	{
		private readonly IProductRepository _productRepository;
		private readonly IComboRepository _comboRepository;
		private readonly ILogger<CreateDiscountRuleCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly IValidator<CreateDiscountRuleDto> _validator;
		private readonly IProductDiscountRuleRepository _productDiscountRuleRepository;
		private readonly IUnitOfWork _unitOfWork;

		public CreateDiscountRuleCommandHandler(
			IProductRepository productRepository,
			IComboRepository comboRepository,
			ILogger<CreateDiscountRuleCommandHandler> logger,
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			IValidator<CreateDiscountRuleDto> validator,
			IProductDiscountRuleRepository productDiscountRuleRepository,
			IUnitOfWork unitOfWork)
		{
			_productRepository = productRepository;
			_comboRepository = comboRepository;
			_logger = logger;
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_validator = validator;
			_productDiscountRuleRepository = productDiscountRuleRepository;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(CreateDiscountRuleCommand request, CancellationToken cancellationToken)
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

				BaseProduct? product = model.IsCombo
					? await _comboRepository.GetByIdAsync(productId)
					: await _productRepository.GetByIdAsync(productId);

				if (product == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"),
						statusCode: 404);
				}

				var discountRule = _mapper.Map<ProductDiscountRule>(model);
				discountRule.ProductId = model.IsCombo ? null : productId;
				discountRule.ComboId = model.IsCombo ? productId : null;
				discountRule.IsCombo = model.IsCombo;

				await _productDiscountRuleRepository.CreateAsync(discountRule);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(discountRule.Id,
					_localizedMessageService.GetLocalizedMessage("Created"), statusCode: 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create new discount rule");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", statusCode: 500);
			}
		}
	}
}