namespace FB98.Modules.Shows.Application.ShowManagement.Create
{
	public class CreateShowDto
	{
		public Guid? MovieId { get; set; }
		public Guid? CinemaHallId { get; set; }
		public DateTime StartTime { get; set; }
		public ICollection<CreateShowFeatureDto> Features { get; set; } = new List<CreateShowFeatureDto>();
	}

	public class CreateShowFeatureDto
	{
		public Guid FeatureId { get; set; }
	}
}