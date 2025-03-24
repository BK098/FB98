using FB98.Modules.Customers.Application.Abstractions;
using FB98.Modules.Customers.DataAccess.Data;
using FB98.Modules.Customers.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Customers.DataAccess.Repositories
{
	public class CustomerRepository : BaseRepository<Customer, CustomerModuleDbContext>, ICustomerRepository
	{
		public CustomerRepository(CustomerModuleDbContext context) : base(context)
		{
		}

		public override async Task<Customer?> GetByIdAsync(Guid? id)
		{
			return await _context.Customers.FirstOrDefaultAsync(x => x.UserId == id);
		}
	}
}