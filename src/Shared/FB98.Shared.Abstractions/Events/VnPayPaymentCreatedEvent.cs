namespace FB98.Shared.Abstractions.Events
{
	public record VnPayPaymentCreatedEvent(Guid UserId, Guid? BookingId, Guid? OrderId);
}