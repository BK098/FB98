using MediatR;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.ReduceStock
{
	public record ReduceStockCommand(ReduceStockDto Model) : ICommand<ApiResult<Unit>>;
}
