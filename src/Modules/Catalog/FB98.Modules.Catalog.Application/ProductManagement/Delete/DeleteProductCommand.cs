namespace FB98.Modules.Catalog.Application.ProductManagement.Delete
{
	public record DeleteProductCommand(Guid ProductId) : ICommand<ApiResult<object>>;
}
