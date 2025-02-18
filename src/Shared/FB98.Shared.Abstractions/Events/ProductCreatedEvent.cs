namespace FB98.Shared.Abstractions.Events
{
	public record ProductCreatedEvent(Guid ProductId, int Quantity, bool IsLimited);
}
