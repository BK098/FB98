namespace FB98.Modules.Tickets.Application.BookingManagement.RetrieveShowSeat
{
	public class RetrieveShowSeatResponse
	{
		public Guid ShowId { get; set; }
		public string MovieTitle { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public Guid HallId { get; set; }
		public List<ShowSeatDto> Seats { get; set; } = new();
	}

	public class ShowSeatDto
	{
		public Guid SeatId { get; set; }
		public Guid SeatTypeId { get; set; }
		public string SeatType { get; set; }
		public string SeatStatus { get; set; }
		public string SeatPosition { get; set; }
	}
}