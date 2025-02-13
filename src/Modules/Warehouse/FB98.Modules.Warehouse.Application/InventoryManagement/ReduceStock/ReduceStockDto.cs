namespace FB98.Modules.Warehouse.Application.InventoryManagement.ReduceStock
{
	public class ReduceStockDto
	{
		public Guid? ProductId { get; set; }
		public int? Quantity { get; set; }
	}
}
