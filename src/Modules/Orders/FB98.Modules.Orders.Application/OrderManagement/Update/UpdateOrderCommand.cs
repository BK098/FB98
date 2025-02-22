namespace FB98.Modules.Orders.Application.OrderManagement.Update
{
	public record UpdateOrderCommand(Guid OrderId, Guid OrderStatusId) : ICommand<ApiResult<object>>;
}
