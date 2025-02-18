using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Orders.Domain.Entities
{
	public class Order : BaseEntity
	{
		public Guid? CustomerId { get; set; }
		public decimal? Amount { get; set; }
		public decimal? SubAmount { get; set; }

		[ForeignKey("OrderStatus")]
		public Guid OrderStatusId { get; set; }
		public OrderStatus OrderStatus { get; set; } = default!;

		public ICollection<OrderItem> OrderItems { get; set; } = new List<OrderItem>();
	}
}
