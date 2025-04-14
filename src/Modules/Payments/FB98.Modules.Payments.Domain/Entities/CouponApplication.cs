using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class CouponApplication : BaseEntity
	{
		public decimal AppliedAmount { get; set; }
		public DateTime AppliedAt { get; set; } = DateTime.UtcNow;

		[ForeignKey("Coupon")]
		public Guid CouponId { get; set; }
		public Coupon? Coupon { get; set; }

		[ForeignKey("PaymentTransaction")]
		public Guid PaymentTransactionId { get; set; }
		public PaymentTransaction? PaymentTransaction { get; set; }
	}
}