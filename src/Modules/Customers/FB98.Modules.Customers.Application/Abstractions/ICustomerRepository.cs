using FB98.Modules.Customers.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Customers.Application.Abstractions
{
	public interface ICustomerRepository : IRepository<Customer>
	{
	}
}