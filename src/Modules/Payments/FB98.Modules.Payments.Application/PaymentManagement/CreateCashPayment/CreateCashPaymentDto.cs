namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	public class CreateCashPaymentDto
	{
		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
		public string? CouponCode { get; set; }
	}
}