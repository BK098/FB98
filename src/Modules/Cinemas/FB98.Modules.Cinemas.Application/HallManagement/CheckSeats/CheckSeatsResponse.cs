namespace FB98.Modules.Cinemas.Application.HallManagement.CheckSeats
{
	public class CheckSeatsResponse
	{
		public string Name { get; set; } = null!;
		public IList<SeatResponse> Seats { get; set; } = null!;
	}

	public class SeatResponse
	{
		public Guid SeatId { get; set; }
		public Guid SeatTypeId { get; set; }
		public string SeatPosition { get; set; } = null!;
	}
}