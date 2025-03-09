namespace FB98.Modules.Tickets.Application.BookingManagement.Create
{
	public record CreateBookingCommand(CreateBookingDto Model) : ICommand<ApiResult<object>>;
}