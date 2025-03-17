using FB98.Modules.Tickets.Domain.Entities;

namespace FB98.Modules.Tickets.Application.Abstractions
{
	public interface IBookingSeatRepository : IRepository<BookingSeat>
	{
		Task<List<BookingSeat>> GetBookedSeatsByShow(Guid showId);
		Task<IEnumerable<BookingSeat>> GetBookingSeatsByStatusAndTimeAsync(Guid statusId, DateTime date);
	}
}