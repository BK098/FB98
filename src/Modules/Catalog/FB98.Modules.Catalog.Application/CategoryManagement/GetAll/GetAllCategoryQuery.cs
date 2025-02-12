using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Catalog.Application.CategoryManagement.GetAll
{
	public record GetAllCategoryQuery(Filter Filter) : IQuery<ApiResponse<PaginatedResult<GetAllCategoryResponse>>>;
}
