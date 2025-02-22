namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	public record CreateCashPaymentCommand(CreateCashPaymentDto Model) : ICommand<ApiResult<object>>;
}