using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Tickets.Application")]
[assembly: InternalsVisibleTo("FB98.Modules.Movies.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface IShowApi
	{
		[Get("/show-module/shows/{showId}")]
		Task<ApiResult<ShowDto>> GetShowById(Guid showId);

		[Get("/show-module/shows/{movieId}/exists")]
		Task<ApiResult<bool>> IsMovieInAnyShow(Guid movieId);
	}

	public record ShowDto(Guid CinemaHallId, string MovieTitle, string StartTime, string EndTime, Guid ShowStatusId, string StartDate);
}