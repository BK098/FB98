using FB98.Modules.Orders.Application.OrderManagement.CheckIn;
using FB98.Modules.Orders.Application.OrderManagement.Create;
using FB98.Modules.Orders.Application.OrderManagement.GetDetail;
using FB98.Modules.Orders.Application.OrderManagement.GetOrderStatusHistory;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Orders.Api.Controllers
{
	internal class OrdersController : BaseController
	{
		public OrdersController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet("{orderId:guid}")]
		public async Task<IActionResult> GetOrder(Guid orderId)
		{
			var request = new GetDetailOrderQuery(orderId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpPost]
		public async Task<IActionResult> CreateOrder([FromBody] CreateOrderDto model)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim != null)
			{
				model.UserId = Guid.Parse(userIdClaim.Value);
			}
			else
			{
				return Unauthorized();
			}

			var request = new CreateOrderCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpGet("{orderId:guid}/history")]
		public async Task<IActionResult> GetOrderStatusHistory(Guid orderId)
		{
			var request = new GetOrderStatusHistoryQuery(orderId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("{orderId:guid}check-in")]
		public async Task<IActionResult> CheckIn(Guid orderId)
		{
			var request = new CheckInCommand(orderId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}