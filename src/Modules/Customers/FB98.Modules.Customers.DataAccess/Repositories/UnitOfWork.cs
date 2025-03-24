using FB98.Modules.Customers.Application.Abstractions;
using FB98.Modules.Customers.DataAccess.Data;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Customers.DataAccess.Repositories
{
	public class UnitOfWork : BaseUnitOfWork<CustomerModuleDbContext>, IUnitOfWork
	{
		public UnitOfWork(CustomerModuleDbContext context) : base(context)
		{
		}
	}
}