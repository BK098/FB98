namespace FB98.Modules.Tickets.Application.SeatManagement.LockSeat
{
	public record LockSeatsCommand(LockSeatsDto Model) : ICommand<ApiResult<object>>;
}