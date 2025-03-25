using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Abstractions.StatusConstants;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class PaymentTransaction : BaseEntity
	{
		public Guid UserId { get; set; }
		public string Email { get; set; }
		public string PhoneNumber { get; set; }
		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
		public decimal Amount { get; set; }
		public string? VNPayTransactionId { get; set; }

		[ForeignKey("PaymentMethod")]
		public Guid PaymentMethodId { get; set; }
		public PaymentMethod PaymentMethod { get; set; }

		[ForeignKey("PaymentStatus")]
		public Guid PaymentStatusId { get; private set; }
		public PaymentStatus PaymentStatus { get; set; }

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