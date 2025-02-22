using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Orders.Domain.Entities
{
	public class OrderItem : BaseEntity
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = null!;
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; } // tiền chưa giảm giá
		public decimal FinalPrice { get; set; } // tiền đã giảm giá
		public decimal SubTotalPrice { get; set; } // tổng tiền của 1 sản phẩm
		public decimal TotalPrice { get; set; } // tổng tiền của 1 sản phẩm sau khi giảm giá

		public bool IsCombo { get; set; } = false;

		[ForeignKey("Order")]
		public Guid OrderId { get; set; }
		public Order Order { get; set; } = default!;
	}
}