using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Tickets.Application.BookingManagement.GetAll
{
	public record GetAllBookingQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllBookingResponse>>>;
}