using FB98.Shared.Abstractions.Events.Base;

namespace FB98.Shared.Abstractions.Events.Products
{
	public class StockResponseEvent : IEvent
	{
		public Guid ProductId { get; }
		public int Quantity { get; }

		public StockResponseEvent(Guid productId, int quantity)
		{
			ProductId = productId;
			Quantity = quantity;
		}
	}
}
