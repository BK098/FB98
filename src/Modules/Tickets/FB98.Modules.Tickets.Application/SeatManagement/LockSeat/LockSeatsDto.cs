namespace FB98.Modules.Tickets.Application.SeatManagement.LockSeat
{
	public class LockSeatsDto
	{
		public Guid? CustomerId { get; set; }
		public Guid ShowId { get; set; }
		public ICollection<Guid> SeatIds { get; set; } = new List<Guid>();
	}
}