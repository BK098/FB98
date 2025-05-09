using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using System.ComponentModel.DataAnnotations;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class PaymentTransaction : BaseEntity
	{
		[StringLength(50)]
		public string? CouponCode { get; set; }
		public Guid UserId { get; set; }

		[StringLength(50)]
		public string Email { get; set; } = null!;

		[StringLength(10)]
		public string PhoneNumber { get; set; } = null!;
		public Guid? OrderId { get; set; }
		public decimal Amount { get; set; }
		public decimal SubAmount { get; set; }
		[StringLength(36)]
		public string? VNPayTransactionId { get; set; }

		[ForeignKey("PaymentMethod")]
		public Guid PaymentMethodId { get; set; }
		public PaymentMethod? PaymentMethod { get; set; }

		[ForeignKey("PaymentStatus")]
		public Guid PaymentStatusId { get; private set; }
		public PaymentStatus? PaymentStatus { get; set; }

		public void MarkSuccess(string? vnpayTransactionId = null)
		{
			PaymentStatusId = PaymentStatusConstants.Success;
			VNPayTransactionId = vnpayTransactionId;
		}

		public void MarkPeding()
		{
			PaymentStatusId = PaymentStatusConstants.Pending;
		}

		public void MarkFailed()
		{
			PaymentStatusId = PaymentStatusConstants.Failed;
		}
	}
}