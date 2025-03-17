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
				.ThenInclude(x => x.SeatStatus)
				.Include(x => x.Status)
				.FirstOrDefaultAsync(x => x.Id == id);
		}
		public async Task<IEnumerable<Booking>> GetBookingsByStatusAndTimeAsync(Guid statusId, DateTime date)
		{
			return await _context.Bookings
				.Where(x => x.StatusId == statusId && x.CreateAt <= date)
				.ToListAsync();
		}
	}
}