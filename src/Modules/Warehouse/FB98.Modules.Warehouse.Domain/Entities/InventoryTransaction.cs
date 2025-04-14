using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Warehouse.Domain.Entities
{
	public class InventoryTransaction : BaseEntity
	{
		[ForeignKey("Inventory")]
		public Guid InventoryId { get; set; }
		public Inventory Inventory { get; set; } = null!;

		public Guid? OrderId { get; set; }
		public Guid ProductId { get; set; }
		public int QuantityChange { get; set; }
		public string TransactionType { get; set; } = null!;
		public bool IsLimited { get; set; } = true;
	}
}