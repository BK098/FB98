using FB98.Modules.Movies.Domain.Enums;

namespace FB98.Modules.Movies.Application.MovieManagement.Create
{
	public class CreateMovieDto
	{
		public string? Title { get; set; }
		public AgeRating AgeRating { get; set; }
		public DateTime? ReleaseDate { get; set; }
		public int? RuntimeMinutes { get; set; }
		public bool IsPublished { get; set; }
		public string? TrailerLink { get; set; }
		public bool? IsVietSub { get; set; }
		public string? Country { get; set; }
		public string? HeaderImageUrl { get; set; }
		public string? PosterImageUrl { get; set; }
		public string? Description { get; set; }
		public string? Language { get; set; }
		public ICollection<CreateMovieGenreDto> Genres { get; set; }
		public ICollection<CreateMovieCastDto> Casts { get; set; }
		public ICollection<CreateMovieDirectorDto> Directors { get; set; }

		public class CreateMovieGenreDto
		{
			public Guid? Id { get; set; }
		}

		public class CreateMovieCastDto
		{
			public Guid? Id { get; set; }
		}

		public class CreateMovieDirectorDto
		{
			public Guid? Id { get; set; }
		}
	}
}