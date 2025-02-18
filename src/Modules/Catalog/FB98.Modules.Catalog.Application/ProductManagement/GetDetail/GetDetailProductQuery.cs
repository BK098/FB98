namespace FB98.Modules.Catalog.Application.ProductManagement.GetDetail
{
	public record GetDetailProductQuery(Guid ProductId) : IQuery<ApiResult<GetDetailProductResponse>>;
}