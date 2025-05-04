namespace FB98.Modules.Catalog.Application.DiscountManagement.UpdateDiscountRule
{
	public record UpdateDiscountRuleCommand(Guid RuleId, UpdateDiscountRuleDto Model) : ICommand<ApiResult<object>>;
}
