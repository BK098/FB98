namespace FB98.Modules.Orders.Application.OrderManagement.GetDetail
{
	public class GetDetailOrderResponse
	{
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public Guid StatusId { get; set; }
	}
}