using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Customers.Domain.Entities
{
	public class Customer : BaseEntity
	{
		public Customer(Guid userId, decimal totalSpent, int loyaltyPoints, Guid membershipId)
		{
			TotalSpent = totalSpent;
			LoyaltyPoints = loyaltyPoints;
			MembershipId = membershipId;
			UserId = userId;
			MemberSince = DateTime.UtcNow;
		}

		public Guid UserId { get; set; }
		public DateTime MemberSince { get; set; }
		public decimal TotalSpent { get; set; }
		public int LoyaltyPoints { get; set; }

		[ForeignKey("Membership")]
		public Guid MembershipId { get; set; }
		public Membership? Membership { get; set; }

		public ICollection<PointTransaction> PointTransactions { get; set; } = new List<PointTransaction>();
	}
}