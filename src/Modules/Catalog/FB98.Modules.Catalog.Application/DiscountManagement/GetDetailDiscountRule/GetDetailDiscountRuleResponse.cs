namespace FB98.Modules.Catalog.Application.DiscountManagement.GetDetailDiscountRule
{
	public class GetDetailDiscountRuleResponse
	{
		public Guid Id { get; set; }
		public string Name { get; set; } = null!;
		public string Description { get; set; } = null!;
		public decimal Value { get; set; }
		public bool IsDiscountPercentage { get; set; }
		public string StartDate { get; set; }
		public string EndDate { get; set; }
	}
}