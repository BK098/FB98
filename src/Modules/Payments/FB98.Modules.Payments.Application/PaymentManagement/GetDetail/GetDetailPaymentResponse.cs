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
		public string ProductName { get; set; } = null!;
		public int Quantity { get; set; }
		public decimal TotalPrice { get; set; }
		public bool IsCombo { get; set; }
	}

	public class GetDeteailBookingPaymentResponse
	{
		public decimal Amount { get; set; }
		public string ShowStart { get; set; } = null!;
		public string MovieTitle { get; set; } = null!;
		public string HallName { get; set; } = null!;
		public IEnumerable<GetDeteailBookingSeatPaymentResponse>? Seats { get; set; }
	}

	public class GetDeteailBookingSeatPaymentResponse
	{
		public string SeatPosition { get; set; } = null!;
		public string SeatTypeName { get; set; } = null!;
		public decimal Price { get; set; }
	}
}