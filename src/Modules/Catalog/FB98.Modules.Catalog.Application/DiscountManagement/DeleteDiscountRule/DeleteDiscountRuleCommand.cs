namespace FB98.Modules.Catalog.Application.DiscountManagement.DeleteDiscountRule
{
	public record DeleteDiscountRuleCommand(Guid RuleId) : ICommand<ApiResult<object>>;
}