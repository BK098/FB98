namespace FB98.Modules.Catalog.Application.DiscountManagement.CreateDiscountRule
{
	public record CreateDiscountRuleCommand(Guid ProductId, CreateDiscountRuleDto Model) : ICommand<ApiResult<object>>;
}