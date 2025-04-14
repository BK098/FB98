namespace FB98.Modules.Payments.Application.CouponManagement.Create
{
	public class CreateCouponDto
	{
		public string? Code { get; set; }
		public decimal? Value { get; set; }
		public decimal? MaxDiscountAmount { get; set; }
		public decimal? MinPaymentAmount { get; set; }
		public string? Description { get; set; }
		public DateTime? StartDate { get; set; }
		public DateTime? EndDate { get; set; }
		public int? MaxUsage { get; set; }
		public bool? IsDiscountPercentage { get; set; }
		public bool? IsActive { get; set; }
		public bool? IsLimited { get; set; }
	}
}