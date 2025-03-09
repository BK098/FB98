using FB98.Modules.Movies.Domain.Enums;
using Microsoft.AspNetCore.Http;

namespace FB98.Modules.Movies.Application.MovieManagement.Update
{
	public class UpdateMovieDto
	{
		public string? Title { get; set; }
		public AgeRating AgeRating { get; set; }
		public DateTime? ReleaseDate { get; set; }
		public int RuntimeMinutes { get; set; }
		public bool? IsPublished { get; set; }
		public string? Country { get; set; }
		public string TrailerLink { get; set; }
		public bool? IsVietSub { get; set; }
		public IFormFile? HeaderImageUrl { get; set; }
		public IFormFile? PosterImageUrl { get; set; }
		public string? Description { get; set; }
		public string? Language { get; set; }

		public ICollection<UpdateMovieGenreDto> Genres { get; set; }
		public ICollection<UpdateMovieCastDto> Casts { get; set; }
		public ICollection<UpdateMovieDirectorDto> Directors { get; set; }
	}

	public class UpdateMovieGenreDto
	{
		public Guid? Id { get; set; }
	}

	public class UpdateMovieCastDto
	{
		public Guid? Id { get; set; }
	}

	public class UpdateMovieDirectorDto
	{
		public Guid? Id { get; set; }
	}
}