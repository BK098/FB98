using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class MovieDirector : BaseEntity
	{
		[ForeignKey("Movie")]
		public Guid MovieId { get; set; }
		public Movie Movie { get; set; }

		[ForeignKey("Director")]
		public Guid DirectorId { get; set; }
		public Director Director { get; set; }
	}
}