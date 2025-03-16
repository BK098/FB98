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
	}

	public record OrderDto(Guid Id, decimal Amount, Guid StatusId);
}