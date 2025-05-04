using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Payments.Application.CouponManagement.GetCouponPublic
{
	public record GetCouponPublicQuery(decimal? Amount) : IQuery<ApiResult<PaginatedResult<GetCouponPublicResponse>>>
	{
	}
}