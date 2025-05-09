namespace FB98.Shared.Abstractions.Events
{
	public record PaymentFailedEvent(Guid? OrderId, string Reason);
}