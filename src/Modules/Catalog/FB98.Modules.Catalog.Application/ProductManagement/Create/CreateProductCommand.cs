namespace FB98.Modules.Catalog.Application.ProductManagement.Create
{
	public record CreateProductCommand(CreateProductDto Model) : ICommand<ApiResult<object>>;
}
