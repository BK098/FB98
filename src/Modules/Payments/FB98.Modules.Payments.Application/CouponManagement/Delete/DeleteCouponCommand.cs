namespace FB98.Modules.Payments.Application.CouponManagement.Delete
{
	public record DeleteCouponCommand(Guid Id) : ICommand<ApiResult<object>>;
}