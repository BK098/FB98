using FB98.Modules.Movies.Domain.Enums;

namespace FB98.Modules.Movies.Application.MovieManagement.GetDetail
{
	public class GetDetailMovieResponse
	{
		public string Title { get; set; }
		public AgeRating AgeRating { get; set; }
		public DateTime ReleaseDate { get; set; }
		public int RuntimeMinutes { get; set; }
		public bool IsPublished { get; set; }
		public string TrailerLink { get; set; }
		public bool? IsVietSub { get; set; }
		public string? HeaderImage { get; set; }
		public string? PosterImage { get; set; }
		public string? Description { get; set; }
		public string? Language { get; set; }
		public string? Country { get; set; }
		public IEnumerable<GetDetailMovieDirectorResponse> Directors { get; set; } = new List<GetDetailMovieDirectorResponse>();
		public IEnumerable<GetDetailMovieGenreResponse> Genres { get; set; } = new List<GetDetailMovieGenreResponse>();
		public IEnumerable<GetDetailMovieCastResponse> Casts { get; set; } = new List<GetDetailMovieCastResponse>();
	}

	public class GetDetailMovieGenreResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class GetDetailMovieCastResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}

	public class GetDetailMovieDirectorResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}