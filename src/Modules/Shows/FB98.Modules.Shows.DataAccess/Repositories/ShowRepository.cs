using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.DataAccess.Data;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.DataAccess.Repositories
{
	public class ShowRepository : BaseRepository<Show, ShowModuleDbContext>, IShowRepository
	{
		public ShowRepository(ShowModuleDbContext context) : base(context)
		{
		}

		public override async Task<Show?> GetByIdAsync(Guid? id)
		{
			return await _context.Shows
				.Include(x => x.ShowStatus)
				.Include(x => x.Features)
				.ThenInclude(x => x.Feature)
				.FirstOrDefaultAsync(x => x.Id == id);
		}
	}
}