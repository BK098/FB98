namespace FB98.Modules.Shows.Application.ShowManagement.Delete
{
	public record DeleteShowCommand(Guid ShowId) : ICommand<ApiResult<object>>;
}