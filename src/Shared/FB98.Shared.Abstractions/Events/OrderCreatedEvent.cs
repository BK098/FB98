namespace FB98.Shared.Abstractions.Events
{
	public record OrderCreatedEvent(Guid OrderId, List<StockItem> StockItems, List<DiscountItem> DiscountItems);
	public record StockItem(Guid ProductId, int Quantity);
	public record DiscountItem(Guid ProductId, bool IsCombo);
}