using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Warehouse.Domain.Entities
{
	public class Inventory : BaseEntity
	{
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
		public int ReservedQuantity { get; set; } = 0;
		/// <summary>
		/// True là sản phẩm có giới hạn số lượng
		/// False là sản phẩm không giới hạn số lượng
		/// </summary>
		public bool IsLimited { get; set; } = true;
		public ICollection<InventoryTransaction> Transactions { get; set; } = new List<InventoryTransaction>();
	}
}