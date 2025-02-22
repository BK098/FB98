namespace FB98.Modules.Orders.Application.OrderManagement.Create
{
	public record CreateOrderCommand(CreateOrderDto Model) : ICommand<ApiResult<object>>;
}