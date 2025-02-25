using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Cinemas.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<CinemaModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(CinemaModuleDbContext context) : base(context)
		{
		}
	}
}