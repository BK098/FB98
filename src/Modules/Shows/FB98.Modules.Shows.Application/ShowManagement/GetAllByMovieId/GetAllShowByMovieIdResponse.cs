namespace FB98.Modules.Shows.Application.ShowManagement.GetAllByMovieId
{
	public class GetAllShowByMovieIdResponse
	{
		public string Date { get; set; }
		public Guid CinemaHallId { get; set; }
		public string CinemaHallName { get; set; }
		public IEnumerable<GetAllShowDto> ShowTimes { get; set; } = new List<GetAllShowDto>();
	}

	public class GetAllShowDto
	{
		public string StartTime { get; set; }
		public string EndTime { get; set; }
		public Guid ShowId { get; set; }
		public Guid ShowStatusId { get; set; }
		public string ShowStatusName { get; set; }
	}
}