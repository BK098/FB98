namespace FB98.Modules.Customers.Application.CustomerManagement.GetDetail
{
	public class GetDetailCustomerResponse
	{
		public Guid UserId { get; set; }
		public DateTime MemberSince { get; set; }
		public decimal TotalSpent { get; set; } = 0;
		public int LoyaltyPoints { get; set; } = 0;
		public Guid MembershipId { get; set; }
		public string Membership { get; set; }
		public int MembershipDiscount { get; set; }
	}
}