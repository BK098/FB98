using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class CategoryRepository : BaseRepository<Category, CatalogModuleDbContext>, ICategoryRepository
	{
		public CategoryRepository(CatalogModuleDbContext context) : base(context)
		{
		}

		public async Task<bool> IsCategoryExistsAsync(string categoryName, CancellationToken cancellationToken)
		{
			return await GetAll().AnyAsync(c => c.Name == categoryName, cancellationToken);
		}

		public override Task<Category?> GetByIdAsync(Guid? id)
		{
			return _context.Categories.Include(x => x.Products).FirstOrDefaultAsync(x => x.Id == id);
		}
	}
}