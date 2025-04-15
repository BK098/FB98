using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Orders.Application")]
[assembly: InternalsVisibleTo("FB98.Modules.Catalog.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	public interface IWarehouseApi
	{
		[Get("/warehouse-module/Inventories/get-stock/{productId}")]
		Task<ApiResult<StockResponse>> GetStock(Guid productId);
	}

	public record StockResponse(Guid ProductId, int Quantity, bool IsLimited);
}