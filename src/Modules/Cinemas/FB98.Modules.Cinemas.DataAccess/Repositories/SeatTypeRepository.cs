using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.DataAccess.Data;
using FB98.Modules.Cinemas.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Cinemas.DataAccess.Repositories
{
	public class SeatTypeRepository : BaseRepository<SeatType, CinemaModuleDbContext>, ISeatTypeRepository
	{
		public SeatTypeRepository(CinemaModuleDbContext context) : base(context)
		{
		}
	}
}