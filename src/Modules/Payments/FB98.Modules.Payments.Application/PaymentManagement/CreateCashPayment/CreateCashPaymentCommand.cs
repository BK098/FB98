using FB98.Shared.Abstractions.Refits;

namespace FB98.Modules.Payments.Application.PaymentManagement.CreateCashPayment
{
	public record CreateCashPaymentCommand(UserDto? Filter, CreateCashPaymentDto Model) : ICommand<ApiResult<object>>;
}