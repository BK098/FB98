using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Customers.Domain.Entities
{
	public class CustomerMembershipBenefit : BaseEntity
	{
		public string LevelName { get; set; } = string.Empty;
		public int DiscountRate { get; set; } = 0;
	}
}
