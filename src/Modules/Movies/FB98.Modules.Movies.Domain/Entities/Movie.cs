using FB98.Modules.Movies.Domain.Enums;
using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class Movie : BaseEntity
	{
		public string Title { get; set; }
		public AgeRating AgeRating { get; set; }
		public DateTime ReleaseDate { get; set; }
		public int RuntimeMinutes { get; set; }
		public bool IsPublished { get; set; }
		public string TrailerLink { get; set; }
		public string? HeaderImage { get; set; }
		public string? PosterImage { get; set; }
		public string? Description { get; set; }
		public string? Language { get; set; }
		public bool IsVietSub { get; set; }
		public string Country { get; set; }

		public ICollection<MovieGenre> Genres { get; set; } = new List<MovieGenre>();
		public ICollection<MovieCast> Casts { get; set; } = new List<MovieCast>();
		public ICollection<MovieDirector> Directors { get; set; } = new List<MovieDirector>();
	}
}