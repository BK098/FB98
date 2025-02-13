namespace FB98.Modules.Catalog.Application.CategoryManagement.Delete
{
	public record DeleteCategoryCommand(Guid CategoryId) : ICommand<ApiResponse<object>>;
}
