namespace FB98.Modules.Payments.Application.PaymentManagement.GetDetail
{
	public class GetDetailPaymentResponse
	{
		public Guid UserId { get; set; }
		public decimal Amount { get; set; }
		public Guid PaymentMethodId { get; set; }
		public string PaymentMethodName { get; set; }
		public Guid PaymentStatusId { get; set; }
		public string PaymentStatusName { get; set; }
		public Guid? OrderId { get; set; }
		public Guid? BookingId { get; set; }
		public DateTime CreateAt { get; set; }
		public IEnumerable<GetDeteailOrderPaymentResponse>? Orders { get; set; }
		public IEnumerable<GetDeteailBookingPaymentResponse>? Tickets { get; set; }
	}

	public class GetDeteailOrderPaymentResponse
	{
		public decimal Amount { get; set; }
		public Guid StatusId { get; set; }
		public IEnumerable<GetDeteailOrderItemPaymentResponse>? Items { get; set; }
	}

	public class GetDeteailOrderItemPaymentResponse
	{
		public string ProductName { get; set; }
		public int Quantity { get; set; }
		public decimal TotalPrice { get; set; }
		public bool IsCombo { get; set; }
	}

	public class GetDeteailBookingPaymentResponse
	{
		public decimal Amount { get; set; }
		public string ShowStart { get; set; }
		public string MovieTitle { get; set; }
		public string HallName { get; set; }
		public IEnumerable<GetDeteailBookingSeatPaymentResponse>? Seats { get; set; }
	}

	public class GetDeteailBookingSeatPaymentResponse
	{
		public string SeatPosition { get; set; }
		public string SeatTypeName { get; set; }
		public decimal Price { get; set; }
	}
}