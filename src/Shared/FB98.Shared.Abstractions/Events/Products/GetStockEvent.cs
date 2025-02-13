using FB98.Shared.Abstractions.Events.Base;

namespace FB98.Shared.Abstractions.Events.Products
{
	public class GetStockEvent : IEvent
	{
		public Guid ProductId { get; }
		public GetStockEvent(Guid productId)
		{
			ProductId = productId;
		}
	}
}
