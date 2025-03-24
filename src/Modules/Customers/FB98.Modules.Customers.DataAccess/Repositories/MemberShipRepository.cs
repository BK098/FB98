using FB98.Modules.Customers.Application.Abstractions;
using FB98.Modules.Customers.DataAccess.Data;
using FB98.Modules.Customers.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Customers.DataAccess.Repositories
{
	public class MemberShipRepository : BaseRepository<Membership, CustomerModuleDbContext>, IMemberShipRepository
	{
		public MemberShipRepository(CustomerModuleDbContext context) : base(context)
		{
		}
	}
}