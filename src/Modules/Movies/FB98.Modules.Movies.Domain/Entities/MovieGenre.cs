using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class MovieGenre : BaseEntity
	{
		[ForeignKey("Movie")]
		public Guid MovieId { get; set; }
		public Movie? Movie { get; set; }

		[ForeignKey("Genre")]
		public Guid GenreId { get; set; }
		public Genre? Genre { get; set; }
	}
}