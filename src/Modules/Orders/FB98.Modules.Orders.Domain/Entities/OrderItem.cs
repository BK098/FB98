using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Orders.Domain.Entities
{
	public class OrderItem : BaseEntity
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = default!;
		public int Quantity { get; set; }
		public decimal Price { get; set; }
		public decimal TotalPrice { get; set; }
		public bool IsCombo { get; set; } = false;

		[ForeignKey("Order")]
		public Guid OrderId { get; set; }
		public Order Order { get; set; } = default!;
	}
}
