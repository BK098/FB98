using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.DataAccess.Data;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;

namespace FB98.Modules.Orders.DataAccess.Repositories
{
	public class OrderRepository : BaseRepository<Order, OrdersModuleDbContext>, IOrderRepository
	{
		public OrderRepository(OrdersModuleDbContext context) : base(context) { }

		public async Task<int> SaveChangesAsync()
		{
			return await _context.SaveChangesAsync();
		}
	}
}
