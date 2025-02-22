using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<CatalogModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(CatalogModuleDbContext context) : base(context)
		{
		}
	}
}