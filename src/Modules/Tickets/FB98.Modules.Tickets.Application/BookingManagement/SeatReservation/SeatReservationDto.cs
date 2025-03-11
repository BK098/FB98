namespace FB98.Modules.Tickets.Application.BookingManagement.SeatReservation
{
	public class SeatReservationDto
	{
		public Guid? CustomerId { get; set; }
		public Guid ShowId { get; set; }
		public IList<Guid> SeatIds { get; set; } = new List<Guid>();
	}
}