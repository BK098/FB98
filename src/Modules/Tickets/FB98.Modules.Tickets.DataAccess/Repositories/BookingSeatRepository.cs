using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Modules.Tickets.Domain.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	public class BookingSeatRepository : BaseRepository<BookingSeat, TicketModuleDbContext>, IBookingSeatRepository
	{
		public BookingSeatRepository(TicketModuleDbContext context) : base(context)
		{
		}

		public async Task<List<BookingSeat>> GetBookedSeatsByShow(Guid showId)
		{
			var bookedSeats = await _context.BookingSeats
				.Include(x => x.Booking)
				.Where(bs => bs.Booking!.ShowId == showId &&
							 (bs.SeatStatusId == BookingSeatStatusConstants.Booked ||
							  bs.SeatStatusId == BookingSeatStatusConstants.CheckIn ||
							  bs.SeatStatusId == BookingSeatStatusConstants.Pending))
				.ToListAsync();

			return bookedSeats;
		}

		public async Task<IEnumerable<BookingSeat>> GetBookingSeatsByStatusAndTimeAsync(Guid statusId, DateTime date)
		{
			return await _context.BookingSeats
				.Where(x => x.SeatStatusId == statusId && x.CreateAt <= date)
				.ToListAsync();
		}
	}
}