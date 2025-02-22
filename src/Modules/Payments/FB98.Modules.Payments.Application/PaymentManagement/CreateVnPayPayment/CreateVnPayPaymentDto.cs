namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	public class CreateVnPayPaymentDto
	{
		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
		public decimal Amount { get; set; }
		public string? IpAddress { get; set; }
	}
}