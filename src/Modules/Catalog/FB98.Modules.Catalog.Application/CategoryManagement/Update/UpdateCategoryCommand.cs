namespace FB98.Modules.Catalog.Application.CategoryManagement.Update
{
	public record UpdateCategoryCommand(Guid CategoryId, UpdateCategoryDto Model) : ICommand<ApiResponse<object>>;
}
