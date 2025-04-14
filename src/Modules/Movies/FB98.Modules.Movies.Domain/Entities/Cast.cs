using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class Cast : BaseEntity
	{
		public string Name { get; set; } = null!;
		public ICollection<MovieCast> MovieCasts { get; set; } = new List<MovieCast>();
	}
}