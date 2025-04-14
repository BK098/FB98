namespace FB98.Modules.Payments.Application.PaymentManagement.ApplyCoupon
{
	public class ApplyCouponResponse
	{
		public string? Code { get; set; }
		public decimal DiscountAmount { get; set; }
		public decimal DiscountApply { get; set; }
	}
}