using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Payments.Application.CouponManagement.GetAll
{
	public record GetAllCouponQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllCouponResponse>>>;
}