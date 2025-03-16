namespace FB98.Modules.Tickets.Application.BookingManagement.CheckIn
{
	public record CheckInCommand(Guid BookingId) : ICommand<ApiResult<object>>;
}