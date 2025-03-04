using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Shows.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface ICinemaApi
	{
		[Get("/cinema-module/halls/{hallId}")]
		Task<ApiResult<CinemaHallDto>> GetHallById(Guid hallId);
	}

	public record CinemaHallDto(Guid Id, string Name);
}