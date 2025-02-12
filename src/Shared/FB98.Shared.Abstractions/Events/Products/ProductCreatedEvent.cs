using FB98.Shared.Abstractions.Events.Base;

namespace FB98.Shared.Abstractions.Events.Products
{
	public class ProductCreatedEvent : IEvent
	{
		public Guid ProductId { get; }
		public int Quantity { get; }

		public ProductCreatedEvent(Guid productId, int quantity)
		{
			ProductId = productId;
			Quantity = quantity;
		}
	}
}
