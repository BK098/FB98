namespace FB98.Modules.Tickets.Application.BookingManagement.SeatReservation
{
	public record SeatReservationCommand(SeatReservationDto Model) : ICommand<ApiResult<object>>;
}