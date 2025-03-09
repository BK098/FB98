using FB98.Modules.Orders.Application.Abstractions;
using FB98.Modules.Orders.DataAccess.Data;
using FB98.Modules.Orders.Domain.Entities;
using FB98.Shared.Infrastructure.Repositpries;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Orders.DataAccess.Repositories
{
	public class OrderRepository : BaseRepository<Order, OrdersModuleDbContext>, IOrderRepository
	{
		public OrderRepository(OrdersModuleDbContext context) : base(context)
		{
		}

		public override Task<Order?> GetByIdAsync(Guid? id)
		{
			return _context.Orders
				.Include(x => x.OrderStatus)
				.Include(x => x.OrderItems)
				.Include(x => x.StatusHistories)
				.FirstOrDefaultAsync(x => x.Id == id);
		}

		public async Task<IEnumerable<Order>> GetOrdersByStatusAndTimeAsync(Guid orderStatusId, DateTime date)
		{
			return await _context.Orders
				.Where(x => x.OrderStatusId == orderStatusId && x.CreateAt <= date)
				.ToListAsync();
		}

		public async Task<IEnumerable<OrderStatusHistory>?> GetOrderStatusHistoryAsync(Guid? orderId)
		{
			return await _context.OrderStatusHistories
				.Where(h => h.OrderId == orderId)
				.OrderByDescending(h => h.CreateAt) // Sắp xếp mới nhất trước
				.ToListAsync();
		}
	}
}