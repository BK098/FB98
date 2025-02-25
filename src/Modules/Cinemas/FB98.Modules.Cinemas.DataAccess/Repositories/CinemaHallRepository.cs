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

		public override Task<CinemaHall?> GetByIdAsync(Guid? id)
		{
			return _context.CinemaHalls
				.Include(x => x.Seats
					.OrderBy(s => s.SeatPosition))
				.ThenInclude(x => x.SeatType)
				.FirstOrDefaultAsync(x => x.Id == id);
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
	}
}