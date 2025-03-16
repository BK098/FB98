namespace FB98.Modules.Orders.Application.OrderManagement.CheckIn
{
	public record CheckInCommand(Guid OrderId) : ICommand<ApiResult<object>>;
}
