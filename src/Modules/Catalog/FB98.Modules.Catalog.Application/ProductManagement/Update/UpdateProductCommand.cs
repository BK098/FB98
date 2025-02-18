namespace FB98.Modules.Catalog.Application.ProductManagement.Update
{
	public record UpdateProductCommand(Guid ProductId, UpdateProductDto Model) : ICommand<ApiResult<object>>;
}
