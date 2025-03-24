namespace FB98.Modules.Tickets.Application.BookingManagement.CheckIn
{
	public record CheckInCommand(CheckInDto Model) : ICommand<ApiResult<object>>;
}