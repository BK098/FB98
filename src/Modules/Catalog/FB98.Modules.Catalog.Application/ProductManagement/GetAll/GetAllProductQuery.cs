using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetAll
{
	public record GetAllProductQuery(Filter Filter) : IQuery<ApiResponse<PaginatedResult<GetAllProductResponse>>>;
}
