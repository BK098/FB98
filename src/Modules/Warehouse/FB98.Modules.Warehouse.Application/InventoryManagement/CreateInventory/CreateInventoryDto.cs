namespace FB98.Modules.Warehouse.Application.InventoryManagement.CreateInventory
{
	public class CreateInventoryDto
	{
		public Guid? ProductId { get; set; }
		public int? InitialStock { get; set; }
	}
}
