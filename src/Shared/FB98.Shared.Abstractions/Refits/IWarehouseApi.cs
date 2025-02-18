using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Orders.Application")]
[assembly: InternalsVisibleTo("FB98.Modules.Catalog.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface IWarehouseApi
	{
		[Get("/warehouse-module/Inventories/get-stock/{productId}")]
		Task<Responses.ApiResult<StockResponse>> GetStock(Guid productId);
	}
	public class StockResponse
	{
		public Guid ProductId { get; set; }
		public int Quantity { get; set; }
		public bool IsLimited { get; set; }
	}
}
