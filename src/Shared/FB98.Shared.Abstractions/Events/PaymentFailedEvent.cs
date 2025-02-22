namespace FB98.Shared.Abstractions.Events
{
	public record PaymentFailedEvent(Guid OrderId, Guid? BookingId, string Reason);
}