namespace FB98.Modules.Payments.Application.CouponManagement.Update
{
	public record UpdateCouponCommand(Guid Id, UpdateCouponDto Model) : ICommand<ApiResult<object>>;
}