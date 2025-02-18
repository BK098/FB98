using FB98.Modules.Orders.Domain.Entities;

namespace FB98.Modules.Orders.Application.Abstractions
{
	public interface IOrderRepository : IRepository<Order>
	{
		Task<int> SaveChangesAsync();
	}
}
