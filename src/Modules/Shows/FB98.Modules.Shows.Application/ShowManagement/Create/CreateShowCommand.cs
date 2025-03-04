namespace FB98.Modules.Shows.Application.ShowManagement.Create
{
	public record CreateShowCommand(CreateShowDto Model) : ICommand<ApiResult<object>>;
}