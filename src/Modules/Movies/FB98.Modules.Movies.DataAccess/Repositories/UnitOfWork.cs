using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Movies.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<MovieModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(MovieModuleDbContext context) : base(context)
		{
		}
	}
}