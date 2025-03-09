using FB98.Modules.Tickets.Application.Abstractions;
using FB98.Modules.Tickets.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Tickets.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<TicketModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(TicketModuleDbContext context) : base(context)
		{
		}
	}
}