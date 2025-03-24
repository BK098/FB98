namespace FB98.Shared.Infrastructure.Payments.VnPay
{
	public interface IVnPayService
	{
		string GeneratePaymentUrl(Guid paymentId, decimal amount, string ipAddress);
		bool ValidateVnPayResponse(SortedDictionary<string, string> queryParams, decimal expectedAmount, string expectedTxnRef);
	}
}
