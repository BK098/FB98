using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Orders.Domain.Entities
{
	public class OrderStatusHistory : BaseEntity
	{
		[ForeignKey("Order")]
		public Guid OrderId { get; set; }
		public Order? Order { get; set; }

		public Guid OldStatusId { get; set; }
		public Guid NewStatusId { get; set; }
		public string? ChangedBy { get; set; }
	}
}