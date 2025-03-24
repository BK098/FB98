using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Customers.Domain.Entities
{
	public class Membership : BaseEntity
	{
		public string LevelName { get; set; } = string.Empty;
		public decimal TotalAmountForUpgrade { get; set; }
		public int DiscountRate { get; set; } = 0;
	}
}