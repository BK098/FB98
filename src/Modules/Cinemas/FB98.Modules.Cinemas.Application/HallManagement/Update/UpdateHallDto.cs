namespace FB98.Modules.Cinemas.Application.HallManagement.Update
{
	public class UpdateHallDto
	{
		public string? Name { get; set; }
		public ICollection<UpdateSeatDto> Seats { get; set; } = new List<UpdateSeatDto>();
	}
	public class UpdateSeatDto
	{
		public Guid SeatId { get; set; }
		public Guid SeatTypeId { get; set; }
	}
}