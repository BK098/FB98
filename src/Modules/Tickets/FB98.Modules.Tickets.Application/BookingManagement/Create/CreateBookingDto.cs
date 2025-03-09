namespace FB98.Modules.Tickets.Application.BookingManagement.Create
{
	public class CreateBookingDto
	{
		public Guid? CustomerId { get; set; }
		public Guid ShowId { get; set; }
		public IList<Guid> SeatIds { get; set; } = new List<Guid>();
	}
}