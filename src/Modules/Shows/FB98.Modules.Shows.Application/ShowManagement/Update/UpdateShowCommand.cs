namespace FB98.Modules.Shows.Application.ShowManagement.Update
{
	public record UpdateShowCommand(Guid ShowId, UpdateShowDto Model) : ICommand<ApiResult<object>>;
}