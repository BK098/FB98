namespace FB98.Modules.Tickets.Application.BookingManagement.GetDetail
{
	public record GetDetailBookingQuery(Guid BookingId) : IQuery<ApiResult<GetDetailBookingResponse>>;
}