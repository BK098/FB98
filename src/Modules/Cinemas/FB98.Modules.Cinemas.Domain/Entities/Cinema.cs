using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Cinemas.Domain.Entities
{
	public class Cinema : BaseEntity
	{
		public string Name { get; set; } = null!;
		public string Address { get; set; } = null!;

		public ICollection<CinemaHall> CinemaHalls { get; set; } = new List<CinemaHall>();
	}
}