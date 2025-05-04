using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Catalog.Application.DiscountManagement.GetAllDiscountRule
{
	public record GetAllDiscountRuleQuery(Filter Filter, bool IsCombo) : IQuery<ApiResult<PaginatedResult<GetAllDiscountRuleResponse>>>;
}