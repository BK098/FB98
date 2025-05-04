using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class ProductDiscountRuleRepository : BaseRepository<ProductDiscountRule, CatalogModuleDbContext>, IProductDiscountRuleRepository
	{
		public ProductDiscountRuleRepository(CatalogModuleDbContext context) : base(context)
		{
		}

		public override async Task<ProductDiscountRule?> GetByIdAsync(Guid? id)
		{
			return await _context.ProductDiscountRules
				.Include(x => x.Product)
				.FirstOrDefaultAsync(x => x.Id == id);
		}
	}
}