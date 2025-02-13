using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.DataAccess.Data;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Catalog.DataAccess.Repositories
{
	public class ComboRepository : BaseRepository<Combo, CatalogModuleDbContext>, IComboRepository
	{
		public ComboRepository(CatalogModuleDbContext context) : base(context)
		{
		}

	}
}
