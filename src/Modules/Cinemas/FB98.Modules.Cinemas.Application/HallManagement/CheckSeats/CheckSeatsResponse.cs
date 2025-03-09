namespace FB98.Modules.Cinemas.Application.HallManagement.CheckSeats
{
	public class CheckSeatsResponse
	{
		public string Name { get; set; }
		public List<SeatReponse> Seats { get; set; }
	}

	public class SeatReponse
	{
		public Guid SeatId { get; set; }
		public Guid SeatTypeId { get; set; }
	}
}