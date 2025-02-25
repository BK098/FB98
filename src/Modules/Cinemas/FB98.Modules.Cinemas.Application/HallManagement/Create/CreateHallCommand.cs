namespace FB98.Modules.Cinemas.Application.HallManagement.Create
{
	public record CreateHallCommand(CreateHallDto Model) : ICommand<ApiResult<object>>;
}