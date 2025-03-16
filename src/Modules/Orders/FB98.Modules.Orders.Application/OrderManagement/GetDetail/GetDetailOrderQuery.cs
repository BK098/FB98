namespace FB98.Modules.Orders.Application.OrderManagement.GetDetail
{
	public record GetDetailOrderQuery(Guid OrderId) : IQuery<ApiResult<GetDetailOrderResponse>>;
}
