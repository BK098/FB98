using FB98.Modules.Customers.Application.CustomerManagement.GetDetail;
using MediatR;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Customers.Api.Controllers
{
	internal class CustomersController : BaseController
	{
		public CustomersController(IMediator mediator) : base(mediator)
		{
		}

		[Authorize]
		[HttpGet]
		public async Task<IActionResult> GetCustomer(Guid? userId)
		{
			Guid? currentUserId = Guid.Parse(User.FindFirstValue(ClaimTypes.NameIdentifier)!);

			if (userId == null || userId == currentUserId)
			{
				userId = currentUserId;
			}
			else if (!User.IsInRole("Administrator"))
			{
				return Forbid();
			}

			var request = new GetDetailCustomerQuery(userId!.Value);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}