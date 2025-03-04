namespace FB98.Modules.Shows.Application.ShowManagement.GetAll
{
	public class GetAllShowResponse
	{
		public Guid MovieId { get; set; }
		public string MovieTitle { get; set; }
		public int MovieRuntimeMinutes { get; set; }
		public Guid CinemaHallId { get; set; }
		public string CinemaHallName { get; set; }
		public IEnumerable<GetAllShowDto> ShowTimes { get; set; } = new List<GetAllShowDto>();
	}

	public class GetAllShowDto
	{
		public Guid ShowId { get; set; }
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public Guid ShowStatusId { get; set; }
		public string ShowStatusName { get; set; }
	}
}