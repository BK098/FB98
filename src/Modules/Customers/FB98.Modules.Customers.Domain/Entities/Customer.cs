using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Customers.Domain.Entities
{
	public class Customer : BaseEntity
	{
		public Guid UserId { get; set; }
		public DateTime MemberSince { get; set; }
		public decimal TotalSpent { get; set; } = 0;
		public int LoyaltyPoints { get; set; } = 0;

		[ForeignKey("CustomerMembershipBenefit")]
		public Guid LevelName { get; set; }
		public CustomerMembershipBenefit CustomerMembershipBenefit { get; set; } = default!;

		//private readonly List<DomainEvent> _events = new();

	}
}
