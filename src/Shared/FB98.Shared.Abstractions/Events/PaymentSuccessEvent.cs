namespace FB98.Shared.Abstractions.Events
{
	public record PaymentSuccessEvent(Guid? OrderId, Guid? BookingId, Guid UserId, decimal Amount, string Email);
}