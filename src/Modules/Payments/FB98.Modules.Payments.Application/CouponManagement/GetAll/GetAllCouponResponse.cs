namespace FB98.Modules.Payments.Application.CouponManagement.GetAll
{
	public class GetAllCouponResponse
	{
		public Guid Id { get; set; }
		public string Code { get; set; } = null!;
		public decimal Value { get; set; }
		public DateTime StartDate { get; set; }
		public DateTime EndDate { get; set; }
		public int MaxUsage { get; set; }
		public int UsageCount { get; set; }
		public bool IsActive { get; set; }
	}
}