namespace FB98.Modules.Cinemas.Application.HallManagement.Update
{
	public record UpdateHallCommand(Guid HallId, UpdateHallDto Model) : ICommand<ApiResult<object>>;
}