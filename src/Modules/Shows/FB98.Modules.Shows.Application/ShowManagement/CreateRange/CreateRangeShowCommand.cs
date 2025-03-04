namespace FB98.Modules.Shows.Application.ShowManagement.CreateRange
{
	public record CreateRangeShowCommand(CreateRangeShowDto Model) : ICommand<ApiResult<object>>;
}