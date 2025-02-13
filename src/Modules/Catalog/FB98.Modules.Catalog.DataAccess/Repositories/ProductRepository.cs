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

		public async Task<bool> IsProductExistsAsync(string productName, CancellationToken cancellationToken)
		{
			return await GetAll().AnyAsync(c => c.Name == productName, cancellationToken);
		}
	}
}
