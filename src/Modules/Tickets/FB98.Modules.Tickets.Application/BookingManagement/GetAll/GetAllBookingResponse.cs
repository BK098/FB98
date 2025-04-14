namespace FB98.Modules.Tickets.Application.BookingManagement.GetAll
{
	public class GetAllBookingResponse
	{
		public Guid UserId { get; set; }
		public string UserName { get; set; } = null!;
		public Guid ShowId { get; set; }
		public Guid HallId { get; set; }
		public string HallName { get; set; } = null!;
		public string ShowStart { get; set; } = null!;
		public string ShowEnd { get; set; } = null!;
		public string MovieTitle { get; set; } = null!;
		public decimal Amount { get; set; }
		public Guid StatusId { get; set; }
		public string StatusName { get; set; } = null!;
	}
}
