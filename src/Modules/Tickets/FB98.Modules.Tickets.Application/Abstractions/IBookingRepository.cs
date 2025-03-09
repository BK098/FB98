using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.Abstractions
{
	public interface IBookingRepository : IRepository<Booking>
	{
		Task<IEnumerable<Guid>> GetUnavailableSeats(Guid showId, IEnumerable<Guid> seatIds);
	}
}