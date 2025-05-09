namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	public class CreateCashPaymentDto
	{
		public Guid? OrderId { get; set; }
		public string? CouponCode { get; set; }
	}
}