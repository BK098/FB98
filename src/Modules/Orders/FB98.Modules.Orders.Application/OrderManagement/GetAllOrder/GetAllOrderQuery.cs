using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Orders.Application.OrderManagement.GetAllOrder
{
	public record GetAllOrderQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllOrderResponse>>>;
}