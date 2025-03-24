namespace FB98.Modules.Payments.Application.PaymentManagement.ProcessVNPayReturn
{
	public record ProcessVnPayReturnCommand(SortedDictionary<string, string> QueryParams, Guid UserId, string Email, string PhoneNumber) : ICommand<ApiResult<string>>;
}