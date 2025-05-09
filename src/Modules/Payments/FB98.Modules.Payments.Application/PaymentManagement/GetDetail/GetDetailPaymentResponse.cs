namespace FB98.Modules.Payments.Application.PaymentManagement.GetDetail
{
	public class GetDetailPaymentResponse
	{
		public Guid UserId { get; set; }
		public decimal Amount { get; set; }
		public Guid PaymentMethodId { get; set; }
		public string PaymentMethodName { get; set; } = null!;
		public Guid PaymentStatusId { get; set; }
		public string PaymentStatusName { get; set; } = null!;
		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
		public DateTime CreateAt { get; set; }
		public IEnumerable<GetDeteailOrderPaymentResponse>? Orders { get; set; }
	}

	public class GetDeteailOrderPaymentResponse
	{
		public decimal Amount { get; set; }
		public Guid StatusId { get; set; }
		public IEnumerable<GetDeteailOrderItemPaymentResponse>? Items { get; set; }
	}

	public class GetDeteailOrderItemPaymentResponse
	{
		public string ProductName { get; set; } = null!;
		public int Quantity { get; set; }
		public decimal TotalPrice { get; set; }
		public bool IsCombo { get; set; }
	}
}