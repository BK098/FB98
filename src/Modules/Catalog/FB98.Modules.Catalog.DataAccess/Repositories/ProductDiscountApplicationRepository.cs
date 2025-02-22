using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class ProductDiscountApplicationRepository : BaseRepository<ProductDiscountApplication, CatalogModuleDbContext>, IProductDiscountApplicationRepository
	{
		public ProductDiscountApplicationRepository(CatalogModuleDbContext context) : base(context)
		{
		}

		public async Task<ProductDiscountRule?> GetCurrentDiscountAsync(Guid productId)
		{
			return await _context.ProductDiscountRules
				.Where(d => d.ProductId == productId && d.StartDate <= DateTime.UtcNow && d.EndDate >= DateTime.UtcNow)
				.OrderByDescending(d => d.StartDate)
				.FirstOrDefaultAsync();
		}
	}
}