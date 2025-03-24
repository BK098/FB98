namespace FB98.Modules.Payments.Application.PaymentManagement.GetPaymentHisotry
{
	public class GetPaymentHisotryResponse
	{
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public string Status { get; set; }
		public string Method { get; set; }
	}
}