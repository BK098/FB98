using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Catalog.Application.DiscountManagement.GetDetailDiscountRule
{
	public record GetDetailDiscountRuleQuery(Guid ProductId, bool IsCombo) : IQuery<ApiResult<PaginatedResult<GetDetailDiscountRuleResponse>>>;
}