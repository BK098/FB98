namespace FB98.Modules.Warehouse.Application.InventoryManagement.AddStock
{
	public class AddStockDto
	{
		public Guid? ProductId { get; set; }
		public int? Quantity { get; set; }
		public bool? IsLimited { get; set; }
	}
}