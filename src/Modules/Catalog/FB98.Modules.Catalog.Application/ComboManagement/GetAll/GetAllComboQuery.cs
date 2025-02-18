using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Catalog.Application.ComboManagement.GetAll
{
	public record GetAllComboQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllComboResponse>>>;
}
