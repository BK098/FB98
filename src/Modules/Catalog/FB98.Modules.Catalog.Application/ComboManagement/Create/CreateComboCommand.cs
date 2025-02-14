namespace FB98.Modules.Catalog.Application.ComboManagement.Create
{
	public record CreateComboCommand(CreateComboDto Model) : ICommand<ApiResponse<object>>;
}
