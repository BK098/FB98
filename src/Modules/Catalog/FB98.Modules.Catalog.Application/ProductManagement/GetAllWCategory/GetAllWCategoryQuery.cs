using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Catalog.Application.ProductManagement.GetAllWCategory
{
	public record GetAllWCategoryQuery(Filter Filter) : IQuery<ApiResult<List<GetAllWCategoryResponse>>>;
}