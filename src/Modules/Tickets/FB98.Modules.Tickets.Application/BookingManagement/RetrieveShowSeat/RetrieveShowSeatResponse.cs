namespace FB98.Modules.Tickets.Application.BookingManagement.RetrieveShowSeat
{
	public class RetrieveShowSeatResponse
	{
		public Guid ShowId { get; set; }
		public string MovieTitle { get; set; } = null!;
		public string StartTime { get; set; } = null!;
		public string EndTime { get; set; } = null!;
		public Guid HallId { get; set; }
		public List<ShowSeatDto> Seats { get; set; } = new();
	}

	public class ShowSeatDto
	{
		public Guid SeatId { get; set; }
		public Guid SeatTypeId { get; set; }
		public string SeatType { get; set; } = null!;
		public string SeatStatus { get; set; } = null!;
		public string SeatPosition { get; set; } = null!;
	}
}