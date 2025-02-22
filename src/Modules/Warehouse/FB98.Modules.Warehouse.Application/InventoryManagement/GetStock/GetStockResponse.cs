using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.GetStock
{
	public class GetStockResponse : IResponse
	{
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
		public bool IsLimited { get; set; }
	}
}