using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.DataAccess.Data;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.DataAccess.Repositories
{
	public class FeatureRepository : BaseRepository<Feature, ShowModuleDbContext>, IFeatureRepository
	{
		public FeatureRepository(ShowModuleDbContext context) : base(context)
		{
		}

		public override async Task<Feature?> GetByIdAsync(Guid? id)
		{
			return await _context.Features.Include(x => x.FeatureType).FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<bool> IsFeatureExistsAsync(string name, CancellationToken cancellationToken)
		{
			return await GetAll().AnyAsync(c => c.Name == name, cancellationToken);
		}
	}
}