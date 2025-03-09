using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.Abstractions
{
	public interface IBookingSeatRepository : IRepository<BookingSeat>
	{
		Task<IEnumerable<Guid>> GetUnavailableSeats(Guid showId, IEnumerable<Guid> seatIds);
	}
}