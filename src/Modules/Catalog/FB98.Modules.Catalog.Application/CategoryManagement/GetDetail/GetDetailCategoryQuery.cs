namespace FB98.Modules.Catalog.Application.CategoryManagement.GetDetail
{
	public record GetDetailCategoryQuery(Guid CategoryId) : IQuery<ApiResult<GetDetailCategoryResponse>>;
}