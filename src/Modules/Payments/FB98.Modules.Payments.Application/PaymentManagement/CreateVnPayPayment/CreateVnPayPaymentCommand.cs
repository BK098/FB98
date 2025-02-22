namespace FB98.Modules.Payments.Application.PaymentManagement.CreateVnPayPayment
{
	public record CreateVnPayPaymentCommand(CreateVnPayPaymentDto Model) : ICommand<ApiResult<string>>;
}