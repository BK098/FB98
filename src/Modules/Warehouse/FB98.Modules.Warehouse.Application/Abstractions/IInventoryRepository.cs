using FB98.Modules.Warehouse.Domain.Entities;

namespace FB98.Modules.Warehouse.Application.Abstractions
{
	public interface IInventoryRepository : IRepository<Inventory>
	{
		Task<bool> Exists(Guid productId);
		Task<Inventory?> GetStock(Guid? productId);
		Task AddStockAsync(Guid productId, int quantity, bool isLimited);
		Task ReduceStock(Guid productId, int quantity);
		Task<bool> RemoveProduct(Guid productId);
		Task ReserveStock(Guid orderId, Guid productId, int quantity);
		Task ReleaseStock(Guid orderId);
		Task StockDeduct(Guid orderId);
	}
}