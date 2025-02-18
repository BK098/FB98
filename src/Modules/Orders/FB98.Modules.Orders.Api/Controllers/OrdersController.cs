using FB98.Modules.Orders.Application.OrderManagement.Create;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Orders.Api.Controllers
{
	internal class OrdersController : BaseController
	{
		public OrdersController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost]
		public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto model)
		{
			var request = new CreateOrderCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}
