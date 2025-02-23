using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Orders.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<OrdersModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(OrdersModuleDbContext context) : base(context)
		{
		}
	}
}
