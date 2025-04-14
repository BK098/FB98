using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Customers.Domain.Entities
{
	public class PointTransaction : BaseEntity
	{
		[ForeignKey("Customer")]
		public Guid CustomerId { get; set; }
		public Customer? Customer { get; set; }

		public int PointChange { get; set; }
		[StringLength(20)]
		public string TransactionType { get; set; } = null!;
		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
	}
}