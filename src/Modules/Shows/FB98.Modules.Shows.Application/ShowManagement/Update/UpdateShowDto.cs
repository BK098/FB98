namespace FB98.Modules.Shows.Application.ShowManagement.Update
{
	public class UpdateShowDto
	{
		public Guid? CinemaHallId { get; set; }
		public Guid? MovieId { get; set; }
		public DateTime? StartTime { get; set; }
		public DateTime? EndTime { get; set; }
		public ICollection<UpdateShowFeatureDto>? Features { get; set; } = new List<UpdateShowFeatureDto>();
	}

	public class UpdateShowFeatureDto
	{
		public Guid? FeatureId { get; set; }
	}
}