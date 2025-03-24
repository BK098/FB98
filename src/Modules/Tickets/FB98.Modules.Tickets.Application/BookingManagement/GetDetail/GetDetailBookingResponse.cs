namespace FB98.Modules.Tickets.Application.BookingManagement.GetDetail
{
	public class GetDetailBookingResponse
	{
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public Guid StatusId { get; set; }
		public string StatusName { get; set; }
		public Guid ShowId { get; set; }
		public string ShowStart { get; set; }
		public string MovieTitle { get; set; }
		public string HallName { get; set; }
		public IEnumerable<GetDetailBookingSeatResponse> Seats { get; set; }
	}
	public class GetDetailBookingSeatResponse
	{
		public Guid SeatId { get; set; }
		public string SeatPosition { get; set; }
		public Guid SeatStatusId { get; set; }
		public string SeatTypeName { get; set; }
		public decimal Price { get; set; }
	}
}