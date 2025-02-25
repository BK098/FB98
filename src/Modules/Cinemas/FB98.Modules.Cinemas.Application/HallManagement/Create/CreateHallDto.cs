namespace FB98.Modules.Cinemas.Application.HallManagement.Create
{
	public class CreateHallDto
	{
		public string? Name { get; set; }
		public Guid? CinemaId { get; set; }
		public int? RangeSeatColumn { get; set; }
		public int? RangeSeatRow { get; set; }
	}
}