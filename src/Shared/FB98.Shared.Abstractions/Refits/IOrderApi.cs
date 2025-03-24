using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Payments.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	internal interface IOrderApi
	{
		[Get("/orders-module/orders/{orderId}")]
		Task<ApiResult<OrderDto>> GetOrderById(Guid orderId);

		[Get("/orders-module/orders/{orderId}")]
		Task<ApiResult<OrderDetailDto>> GetOrderDetailById(Guid orderId);
	}

	public record OrderDto(Guid Id, decimal Amount, Guid StatusId);
	public record OrderDetailDto(Guid Id, decimal Amount, Guid StatusId, IEnumerable<OrderDetailItemDto> Items);
	public record OrderDetailItemDto(string ProductName, int Quantity, decimal TotalPrice, bool IsCombo);
}