using MediatR;

namespace FB98.Modules.Warehouse.Application.InventoryManagement.CreateInventory
{
	public record CreateInventoryCommand(CreateInventoryDto Model) : ICommand<ApiResponse<Unit>>;
}
