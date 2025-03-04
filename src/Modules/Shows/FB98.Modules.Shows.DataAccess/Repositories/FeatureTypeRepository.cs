using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.DataAccess.Data;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.DataAccess.Repositories
{
	internal class FeatureTypeRepository : BaseRepository<FeatureType, ShowModuleDbContext>, IFeatureTypeRepository
	{
		public FeatureTypeRepository(ShowModuleDbContext context) : base(context)
		{
		}
		public async Task<bool> IsFeatureTypeExistsAsync(string name, CancellationToken cancellationToken)
		{
			return await GetAll().AnyAsync(c => c.Name == name, cancellationToken);
		}
	}
}