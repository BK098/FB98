using FB98.Modules.Catalog.Application.Abstractions;

namespace FB98.Modules.Catalog.Application.DiscountManagement.DeleteDiscountRule
{
	internal sealed class DeleteDiscountRuleCommandHandler : ICommandHandler<DeleteDiscountRuleCommand, ApiResult<object>>
	{
		private readonly IProductDiscountRuleRepository _discountRuleRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<DeleteDiscountRuleCommandHandler> _logger;
		private readonly IUnitOfWork _unitOfWork;


		public DeleteDiscountRuleCommandHandler(
			IProductDiscountRuleRepository discountRuleRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<DeleteDiscountRuleCommandHandler> logger,
			IUnitOfWork unitOfWork)
		{
			_discountRuleRepository = discountRuleRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_unitOfWork = unitOfWork;
		}

		public async Task<ApiResult<object>> Handle(DeleteDiscountRuleCommand request, CancellationToken cancellationToken)
		{
			var ruleId = request.RuleId;
			try
			{
				var rule = await _discountRuleRepository.GetByIdAsync(ruleId);
				if (rule is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_discountRuleRepository.Delete(rule);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>("", _localizedMessageService.GetLocalizedMessage("Deleted"));
			}
			catch (InvalidOperationException ex)
			{
				_logger.LogWarning(ex, "Error occurred while deleting discount rule");
				return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("DeleteFailedLinked"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while deleting discount rule");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}