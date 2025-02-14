namespace FB98.Modules.Catalog.Application.ComboManagement.Update
{
	public record UpdateComboCommand(Guid ComboId, UpdateComboDto Model) : ICommand<ApiResponse<object>>;
}
