namespace FB98.Modules.Payments.Application.CouponManagement.GetDetail
{
	public record GetDetailCouponQuery(Guid CouponId) : IQuery<ApiResult<GetDetailCouponResponse>>;
}