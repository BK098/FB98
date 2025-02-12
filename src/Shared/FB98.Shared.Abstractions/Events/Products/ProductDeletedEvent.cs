using FB98.Shared.Abstractions.Events.Base;

namespace FB98.Shared.Abstractions.Events.Products
{
	public class ProductDeletedEvent : IEvent
	{
		public Guid ProductId { get; }
		public ProductDeletedEvent(Guid productId)
		{
			ProductId = productId;
		}
	}
}
