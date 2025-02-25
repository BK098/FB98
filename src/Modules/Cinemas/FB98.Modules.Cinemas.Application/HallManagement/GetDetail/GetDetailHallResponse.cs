namespace FB98.Modules.Cinemas.Application.HallManagement.GetDetail
{
	public class GetDetailHallResponse
	{
		public string Name { get; set; }
		public int SeatsCount { get; set; }
		public IEnumerable<GetDetailSeatDto> Seats { get; set; } = new List<GetDetailSeatDto>();
	}

	public class GetDetailSeatDto
	{
		public Guid SeatId { get; set; }
		public string SeatType { get; set; }
		public string SeatPosition { get; set; }
	}
}