using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Cinemas.Domain.Entities
{
	public class Cinema : BaseEntity
	{
		public string Name { get; set; }
		public string Address { get; set; }

		public ICollection<CinemaHall> CinemaHalls { get; set; } = new List<CinemaHall>();
	}
}