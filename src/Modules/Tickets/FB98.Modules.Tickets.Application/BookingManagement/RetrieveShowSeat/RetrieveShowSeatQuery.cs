namespace FB98.Modules.Tickets.Application.BookingManagement.RetrieveShowSeat
{
	public record RetrieveShowSeatQuery(Guid ShowId) : IQuery<ApiResult<RetrieveShowSeatResponse>>;
}