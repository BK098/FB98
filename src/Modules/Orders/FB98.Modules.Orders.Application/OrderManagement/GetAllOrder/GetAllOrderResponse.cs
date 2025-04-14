namespace FB98.Modules.Orders.Application.OrderManagement.GetAllOrder
{
	public class GetAllOrderResponse
	{
		public Guid Id { get; set; }
		public Guid? UserId { get; init; }
		public decimal DiscountPercentage { get; set; }
		public decimal Amount { get; set; }
		public Guid OrderStatusId { get; set; }
		public string? OrderStatusName { get; set; }
		public string CreateAt { get; set; }
	}
}