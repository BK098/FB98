using MediatR;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.AddStock
{
	public record AddStockCommand(AddStockDto Model) : ICommand<ApiResult<Unit>>;
}
