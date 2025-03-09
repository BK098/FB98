using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.DataAccess.Data;
using FB98.Modules.Cinemas.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Cinemas.DataAccess.Repositories
{
	public class CinemaRepository : BaseRepository<Cinema, CinemaModuleDbContext>, ICinemaRepository
	{
		public CinemaRepository(CinemaModuleDbContext context) : base(context)
		{
		}

		public override async Task<Cinema?> GetByIdAsync(Guid? id)
		{
			return await _context.Cinemas
				.Include(x => x.CinemaHalls)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<bool> IsCinemaExisted(string cinemaName)
		{
			return await _context.Cinemas.AnyAsync(x => x.Name == cinemaName);
		}
	}
}