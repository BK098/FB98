using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	public class BookingRepository : BaseRepository<Booking, TicketModuleDbContext>, IBookingRepository
	{
		public BookingRepository(TicketModuleDbContext context) : base(context)
		{
		}

		public override async Task<Booking?> GetByIdAsync(Guid? id)
		{
			return await _context.Bookings
				.Include(x => x.BookingSeats)
				.ThenInclude(x => x.Status)
				.Include(x => x.Status)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<IEnumerable<Guid>> GetUnavailableSeats(Guid showId, IEnumerable<Guid> seatIds)
		{
			return await _context.BookingSeats
				.Where(s => s.ShowId == showId && seatIds.Contains(s.SeatId) && s.StatusId != BookingSeatStatusConstants.Available) // "Available" là trạng thái của ghế
				.Select(s => s.SeatId)
				.ToListAsync();
		}
	}
}