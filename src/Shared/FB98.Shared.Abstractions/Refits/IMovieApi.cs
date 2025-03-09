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

	public record MovieDto(string Title, int RuntimeMinutes, string? PosterImage);
}