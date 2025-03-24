namespace FB98.Modules.Payments.Application.PaymentManagement.GetDetail
{
	public record GetDetailPaymentQuery(Guid PaymentId) : IQuery<ApiResult<GetDetailPaymentResponse>>;
}
