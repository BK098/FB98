using FB98.Modules.Warehouse.Domain.Entities;

namespace FB98.Modules.Warehouse.Application.Abstractions
{
	public interface IInventoryRepository : IRepository<Inventory>
	{
		Task<bool> Exists(Guid productId);
		Task<int> GetStock(Guid productId);
		Task AddStockAsync(Guid productId, int quantity);
		Task ReduceStock(Guid productId, int quantity);
		Task<bool> RemoveProduct(Guid productId);
	}
}
