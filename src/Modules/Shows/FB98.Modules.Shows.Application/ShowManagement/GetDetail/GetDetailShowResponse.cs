namespace FB98.Modules.Shows.Application.ShowManagement.GetDetail
{
	public class GetDetailShowResponse
	{
		public Guid MovieId { get; set; }
		public string MovieTitle { get; set; }
		public int MovieRuntimeMinutes { get; set; }
		public Guid CinemaHallId { get; set; }
		public string CinemaHallName { get; set; }
		public DateTime StartTime { get; set; }
		public DateTime EndTime { get; set; }
		public Guid ShowStatusId { get; set; }
		public string ShowStatusName { get; set; }

		public IEnumerable<GetDetailShowFeatureResponse> Features { get; set; } = new List<GetDetailShowFeatureResponse>();
	}

	public class GetDetailShowFeatureResponse
	{
		public Guid Id { get; set; }
		public string FeatureName { get; set; }
	}
}