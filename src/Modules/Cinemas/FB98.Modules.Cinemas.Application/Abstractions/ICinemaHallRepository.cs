using FB98.Modules.Cinemas.Domain.Entities;

namespace FB98.Modules.Cinemas.Application.Abstractions
{
	public interface ICinemaHallRepository : IRepository<CinemaHall>
	{
		Task<bool> IsCinemaHallExisted(Guid cinemaId, string cinemaName);
		Task<bool> AddRangeSeatsAsync(IEnumerable<CinemaHallSeat> seats);
	}
}