using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Cinemas.Application.CinemaManagement.GetDetail
{
	public class GetDetailCinemaResponse : IResponse
	{
		public Guid Id { get; set; }
		public string Address { get; set; } = null!;
		public int HallsCount { get; set; }
		public IEnumerable<HallDto>? Halls { get; set; } = new List<HallDto>();
	}

	public class HallDto
	{
		public Guid HallId { get; set; }
		public string Name { get; set; } = null!;
	}
}