using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class ComboRepository : BaseRepository<Combo, CatalogModuleDbContext>, IComboRepository
	{
		public ComboRepository(CatalogModuleDbContext context) : base(context)
		{
		}

		public override async Task<Combo?> GetByIdAsync(Guid? id)
		{
			return await _context.Combos
				.Include(x => x.ComboProducts)
				.ThenInclude(x => x.Product)
				.Include(x => x.DiscountRules)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public override async Task<List<Combo>> GetByIdsAsync(List<Guid> ids)
		{
			return await _context.Combos
				.Where(x => ids.Contains(x.Id))
				.Include(x => x.DiscountRules)
				.ToListAsync();
		}
	}
}