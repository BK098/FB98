namespace FB98.Modules.Orders.Application.OrderManagement.GetDetail
{
	public class GetDetailOrderResponse
	{
		public Guid Id { get; set; }
		public decimal Amount { get; set; }
		public Guid StatusId { get; set; }
		public IEnumerable<GetDetailOrderItemResponse> Items { get; set; } = new List<GetDetailOrderItemResponse>();
	}

	public class GetDetailOrderItemResponse
	{
		public Guid ProductId { get; set; }
		public string ProductName { get; set; } = null!;
		public int Quantity { get; set; }
		public decimal UnitPrice { get; set; }
		public decimal FinalPrice { get; set; }
		public decimal SubTotalPrice { get; set; }
		public decimal TotalPrice { get; set; }
		public bool IsCombo { get; set; }
	}
}