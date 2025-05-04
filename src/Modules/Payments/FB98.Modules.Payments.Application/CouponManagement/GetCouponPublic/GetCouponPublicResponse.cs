namespace FB98.Modules.Payments.Application.CouponManagement.GetCouponPublic
{
	public class GetCouponPublicResponse
	{
		public string Code { get; set; } = null!;
		public string? Description { get; set; }
		public object Value { get; set; } = null!;
		public string StartDate { get; set; } = null!;
		public string EndDate { get; set; } = null!;
		public int MaxUsage { get; set; }
		public bool IsDiscountPercentage { get; set; }
		public decimal? MaxDiscountAmount { get; set; }
		public decimal? MinPaymentAmount { get; set; }
	}
}