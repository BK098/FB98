namespace FB98.Modules.Shows.Application.ShowManagement.CreateRange
{
	public class CreateRangeShowDto
	{
		public Guid? MovieId { get; set; }
		public Guid? CinemaHallId { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int? TimeRest { get; set; }
		public ICollection<CreateRangeShowFeatureDto>? Features { get; set; } = new List<CreateRangeShowFeatureDto>();
	}

	public class CreateRangeShowFeatureDto
	{
		public Guid? FeatureId { get; set; }
	}
}