namespace FB98.Shared.Abstractions.Events
{
	public record PaymentCreatedEvent(Guid UserId, Guid? OrderId);
}