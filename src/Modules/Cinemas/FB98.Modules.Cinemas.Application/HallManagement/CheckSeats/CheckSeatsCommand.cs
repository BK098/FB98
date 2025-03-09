namespace FB98.Modules.Cinemas.Application.HallManagement.CheckSeats
{
	public record CheckSeatsCommand(Guid HallId, CheckSeatsDto Model) : ICommand<ApiResult<CheckSeatsResponse>>;
}