using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class ProductDiscountRuleRepository : BaseRepository<ProductDiscountRule, CatalogModuleDbContext>, IProductDiscountRuleRepository
	{
		public ProductDiscountRuleRepository(CatalogModuleDbContext context) : base(context)
		{
		}
	}
}