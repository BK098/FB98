namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	public record CreateCashPaymentCommand(string? SearchTerm, CreateCashPaymentDto Model) : ICommand<ApiResult<object>>;
}