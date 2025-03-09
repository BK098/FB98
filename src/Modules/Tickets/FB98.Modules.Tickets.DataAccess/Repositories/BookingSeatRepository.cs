using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	public class BookingSeatRepository : BaseRepository<BookingSeat, TicketModuleDbContext>, IBookingSeatRepository
	{
		public BookingSeatRepository(TicketModuleDbContext context) : base(context)
		{
		}

		public Task<IEnumerable<Guid>> GetUnavailableSeats(Guid showId, IEnumerable<Guid> seatIds)
		{
			throw new NotImplementedException();
		}
	}
}
