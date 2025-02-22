namespace FB98.Modules.Orders.Application.OrderManagement.GetOrderStatusHistory
{
	public record GetOrderStatusHistoryQuery(Guid OrderId) : IQuery<ApiResult<IEnumerable<GetOrderStatusHistoryResponse>>>;
}