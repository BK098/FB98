using FB98.Modules.Movies.Domain.Enums;

namespace FB98.Modules.Movies.Application.MovieManagement.GetAll
{
	public class GetAllMovieResponse
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public AgeRating AgeRating { get; set; }
		public DateTime ReleaseDate { get; set; }
		public int RuntimeMinutes { get; set; }
		public bool IsPublished { get; set; }
		public string TrailerLink { get; set; }
		public string? PosterImage { get; set; }
		public IEnumerable<GetAllMovieGenreResponse> Genres { get; set; }
	}

	public class GetAllMovieGenreResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; }
	}
}