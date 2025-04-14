namespace FB98.Modules.Payments.Application.PaymentManagement.ApplyCoupon
{
	public record ApplyCouponCommand(ApplyCouponDto Model) : ICommand<ApiResult<ApplyCouponResponse>>;
}