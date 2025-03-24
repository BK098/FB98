namespace FB98.Modules.Tickets.Application.BookingManagement.CheckIn
{
	public class CheckInDto
	{
		public Guid BookingId { get; set; }
		public IList<Guid> SeatIds { get; set; }
	}
}