using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Orders.Domain.Entities
{
	public class Order : BaseEntity
	{
		public Guid? CustomerId { get; init; }
		public decimal DiscountPercentage { get; private set; } // phần trăm giảm giá cho cả đơn hàng
		public decimal Amount { get; set; }
		public decimal SubAmount { get; set; }

		[ForeignKey("OrderStatus")]
		public Guid OrderStatusId { get; set; }
		public OrderStatus OrderStatus { get; set; }

		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
		public ICollection<OrderStatusHistory>? StatusHistories { get; set; } = new List<OrderStatusHistory>();

		public void SetDiscountPercentage()
		{
			if (SubAmount == 0)
			{
				DiscountPercentage = 0;
				return;
			}

			var discountPrercentage = (SubAmount - Amount) / SubAmount * 100;
			DiscountPercentage = Math.Round(discountPrercentage, 2);
		}
	}
}