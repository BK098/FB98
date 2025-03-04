using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Shows.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface IMovieApi
	{
		[Get("/movie-module/movies/{movieId}")]
		Task<ApiResult<MovieDto>> GetMovieById(Guid movieId);
	}

	public class MovieDto
	{
		public Guid Id { get; set; }
		public string Title { get; set; }
		public int RuntimeMinutes { get; set; }
		public string? PosterImage { get; set; }
	}
}