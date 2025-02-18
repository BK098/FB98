namespace FB98.Shared.Abstractions.Events
{
	public record StockDeductedEvent(Guid OrderId, List<StockItem> Items);

	public record StockItem(Guid ProductId, int Quantity);
}
