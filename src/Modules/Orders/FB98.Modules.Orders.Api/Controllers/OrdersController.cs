using FB98.Modules.Orders.Application.OrderManagement.Create;
using FB98.Modules.Orders.Application.OrderManagement.GetOrderStatusHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Orders.Api.Controllers
{
	internal class OrdersController : BaseController
	{
		public OrdersController(IMediator mediator) : base(mediator)
		{
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto model)
		{
			var request = new CreateOrderCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpGet("{orderId}/history")]
		public async Task<IActionResult> GetOrderStatusHistory([FromQuery] Guid orderId)
		{
			var request = new GetOrderStatusHistoryQuery(orderId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}