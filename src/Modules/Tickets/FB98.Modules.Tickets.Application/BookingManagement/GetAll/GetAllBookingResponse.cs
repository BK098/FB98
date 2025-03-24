namespace FB98.Modules.Tickets.Application.BookingManagement.GetAll
{
	public class GetAllBookingResponse
	{
		public Guid UserId { get; set; }
		public string UserName { get; set; }
		public Guid ShowId { get; set; }
		public Guid HallId { get; set; }
		public string HallName { get; set; }
		public string ShowStart { get; set; }
		public string ShowEnd { get; set; }
		public string MovieTitle { get; set; }
		public decimal Amount { get; set; }
		public Guid StatusId { get; set; }
		public string StatusName { get; set; }
	}
}
