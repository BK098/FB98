namespace FB98.Modules.Payments.Application.PaymentManagement.ProcessVNPayReturn
{
	public record ProcessVnPayReturnCommand(SortedDictionary<string, string> QueryParams) : ICommand<ApiResult<string>>;
}