using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Shows.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<ShowModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(ShowModuleDbContext context) : base(context)
		{
		}
	}
}