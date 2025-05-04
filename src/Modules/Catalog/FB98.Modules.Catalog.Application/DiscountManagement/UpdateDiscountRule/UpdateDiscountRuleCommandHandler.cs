using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.DiscountManagement.UpdateDiscountRule
{
	internal sealed class UpdateDiscountRuleCommandHandler : ICommandHandler<UpdateDiscountRuleCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateDiscountRuleCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IProductDiscountRuleRepository _productDiscountRuleRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateDiscountRuleDto> _validator;

		public UpdateDiscountRuleCommandHandler(
			IComboRepository comboRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<UpdateDiscountRuleCommandHandler> logger,
			IMapper mapper,
			IProductDiscountRuleRepository productDiscountRuleRepository,
			IProductRepository productRepository,
			IUnitOfWork unitOfWork,
			IValidator<UpdateDiscountRuleDto> validator)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_productDiscountRuleRepository = productDiscountRuleRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
		}

		public async Task<ApiResult<object>> Handle(UpdateDiscountRuleCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var productId = request.RuleId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var rule = await _productDiscountRuleRepository.GetByIdAsync(productId);
				if (rule == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_mapper.Map(model, rule);

				rule.StartDate = model.StartDate.ToUniversalTime();
				rule.EndDate = model.EndDate.ToUniversalTime();
				rule.IsCombo = model.IsCombo!.Value;
				_productDiscountRuleRepository.Update(rule);
				//await _productDiscountRuleRepository.CreateAsync(rule);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(rule.Id, _localizedMessageService.GetLocalizedMessage("Updated"), 200);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create new discount rule");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}