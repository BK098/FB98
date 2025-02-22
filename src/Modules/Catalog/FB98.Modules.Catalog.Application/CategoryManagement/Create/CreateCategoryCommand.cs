namespace FB98.Modules.Catalog.Application.CategoryManagement.Create
{
	public record CreateCategoryCommand(CreateCategoryDto Model) : ICommand<ApiResult<object>>;
}