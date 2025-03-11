using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.DataAccess.Data;
using FB98.Modules.Cinemas.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Cinemas.DataAccess.Repositories
{
	public class CinemaHallRepository : BaseRepository<CinemaHall, CinemaModuleDbContext>, ICinemaHallRepository
	{
		public CinemaHallRepository(CinemaModuleDbContext context) : base(context)
		{
		}

		public override async Task<CinemaHall?> GetByIdAsync(Guid? id)
		{
			var hall = await _context.CinemaHalls
				.Include(x => x.Seats)
				.ThenInclude(x => x.SeatType)
				.FirstOrDefaultAsync(x => x.Id == id);

			if (hall != null)
			{
				hall.Seats = hall.Seats.OrderBy(s => s.SeatPosition).ToList();
			}

			return hall;
		}

		public async Task<bool> IsCinemaHallExisted(Guid cinemaId, string cinemaName)
		{
			return await _context.CinemaHalls.AnyAsync(x => x.CinemaId == cinemaId && x.Name == cinemaName);
		}

		public async Task<bool> AddRangeSeatsAsync(IEnumerable<CinemaHallSeat> seats)
		{
			await _context.CinemaHallSeats.AddRangeAsync(seats);
			return true;
		}

		public async Task<CinemaHall?> GetValidHallSeats(Guid? hallId, List<Guid> seatIds)
		{
			return await _context.CinemaHalls
				.AsNoTracking()
				.Include(h => h.Seats.Where(s => seatIds.Contains(s.Id)))
				.FirstOrDefaultAsync(h => h.Id == hallId);
		}
	}
}