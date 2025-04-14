namespace FB98.Modules.Cinemas.Application.HallManagement.GetDetail
{
	public class GetDetailHallResponse
	{
		public string Name { get; set; } = null!;
		public int SeatsCount { get; set; }
		public int MaxSeatRow { get; set; }
		public int MaxSeatColumn { get; set; }
		public IEnumerable<GetDetailSeatDto> Seats { get; set; } = new List<GetDetailSeatDto>();
	}

	public class GetDetailSeatDto
	{
		public Guid SeatId { get; set; }
		public Guid SeatTypeId { get; set; }
		public string SeatType { get; set; } = null!;
		public string SeatPosition { get; set; } = null!;
	}
}