using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Shows.Application")]
[assembly: InternalsVisibleTo("FB98.Modules.Tickets.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface ICinemaApi
	{
		[Get("/cinema-module/halls/{hallId}")]
		Task<ApiResult<CinemaHallDto>> GetHallById(Guid hallId);

		[Post("/cinema-module/halls/{hallId}/check-seats")]
		Task<ApiResult<CheckSeatsResponse>> CheckSeats(Guid hallId, [Body] CheckSeastsDto model);
	}

	public record CinemaHallDto(string Name, IList<CinemaHallSeatDto> Seats);

	public record CinemaHallSeatDto(Guid SeatId, string SeatType, string SeatPosition);

	public record CheckSeatsResponse(string Name, IList<Dictionary<Guid, Guid>> SeatIds);

	public record CheckSeastsDto(IList<Guid> SeatIds);
}