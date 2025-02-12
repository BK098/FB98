namespace FB98.Modules.Warehouse.Application.InventoryManagement.GetStock
{
	public record GetStockQuery(Guid ProductId) : IQuery<ApiResponse<int>>;
}
