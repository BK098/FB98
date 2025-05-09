namespace FB98.Shared.Abstractions.Events
{
	public record PaymentSuccessEvent(Guid? OrderId, Guid UserId, decimal Amount, string Email);
}