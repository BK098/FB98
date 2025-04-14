using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class MovieCast : BaseEntity
	{
		[ForeignKey("Movie")]
		public Guid MovieId { get; set; }
		public Movie? Movie { get; set; }

		[ForeignKey("Cast")]
		public Guid CastId { get; set; }
		public Cast? Cast { get; set; }
	}
}