namespace FB98.Modules.Payments.Application.CouponManagement.Create
{
	public record CreateCouponCommand(CreateCouponDto Model) : ICommand<ApiResult<object>>;
}