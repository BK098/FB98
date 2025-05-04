namespace FB98.Modules.Catalog.Application.DiscountManagement.GetAllDiscountRule
{
	public class GetAllDiscountRuleResponse
	{
		public Guid ProductId { get; set; }
		public string? ProductName { get; set; }
		public string? ProductImage { get; set; }
		public Guid? CurrentRuleId { get; set; }
		public string? CurrentRule { get; set; }
		public string? UntilEnd { get; set; }
		public int TotalRule { get; set; }
	}
}