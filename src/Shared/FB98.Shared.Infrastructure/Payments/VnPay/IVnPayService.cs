namespace FB98.Shared.Infrastructure.Payments.VnPay
{
	public interface IVnPayService
	{
		string GeneratePaymentUrl(Guid? orderId, Guid? bookingId, decimal amount, string ipAddress);
		bool ValidateVnPayResponse(SortedDictionary<string, string> queryParams);
	}
}
