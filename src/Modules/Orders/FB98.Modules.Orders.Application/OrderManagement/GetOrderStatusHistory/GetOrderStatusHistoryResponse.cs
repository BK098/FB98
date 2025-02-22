using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Orders.Application.OrderManagement.GetOrderStatusHistory
{
	public class GetOrderStatusHistoryResponse : IResponse
	{
		public Guid OldStatusId { get; set; }
		public Guid NewStatusId { get; set; }
		public string OldStatus { get; set; }
		public string NewStatus { get; set; }
		public DateTime ChangedAt { get; set; }
		//public string ChangedBy { get; set; }
	}
}