using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class Genre : BaseEntity
	{
		public string Name { get; set; }
		public string? Description { get; set; }
	}
}