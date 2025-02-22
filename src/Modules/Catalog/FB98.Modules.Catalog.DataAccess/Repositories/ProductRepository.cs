using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class ProductRepository : BaseRepository<Product, CatalogModuleDbContext>, IProductRepository
	{
		public ProductRepository(CatalogModuleDbContext context) : base(context)
		{
		}

		public override async Task<Product?> GetByIdAsync(Guid? id)
		{
			return await _context.Products
				.Include(x => x.Category)
				.Include(x => x.DiscountRules)
				.FirstOrDefaultAsync(x => x.Id.Equals(id));
		}

		public override async Task<List<Product>> GetByIdsAsync(List<Guid> ids)
		{
			return await _context.Products
				.Where(x => ids.Contains(x.Id))
				.Include(x => x.DiscountRules)
				.ToListAsync();
		}

		public async Task<bool> IsProductExistsAsync(string productName, CancellationToken cancellationToken)
		{
			return await GetAll().AnyAsync(c => c.Name == productName, cancellationToken);
		}
	}
}