namespace FB98.Modules.Catalog.Application.ComboManagement.Delete
{
	public record DeleteComboCommand(Guid ComboId) : ICommand<ApiResult<object>>;
}
