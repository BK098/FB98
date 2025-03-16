using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Tickets.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface IShowApi
	{
		[Get("/show-module/shows/{showId}")]
		Task<ApiResult<ShowDto>> GetShowById(Guid showId);
	}

	public record ShowDto(Guid CinemaHallId, string MovieTitle, DateTime StartTime, DateTime EndTime, Guid ShowStatusId);
}