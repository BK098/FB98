using FB98.Modules.Tickets.Application.SeatManagement.LockSeat;
using FB98.Modules.Tickets.Application.SeatManagement.UnlockSeat;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Tickets.Api.Controllers
{
	internal class SeatsController : BaseController
	{
		public SeatsController(IMediator mediator) : base(mediator)
		{
		}

		[Authorize]
		[HttpPost("lock-seats")]
		public async Task<IActionResult> LockSeats([FromBody] LockSeatsDto model)
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

			var request = new LockSeatsCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpPost("unlock-seats")]
		public async Task<IActionResult> UnLockSeats([FromBody] UnlockSeatsDto model)
		{
			var userIdClaim = User.FindFirst(ClaimTypes.NameIdentifier);
			if (userIdClaim != null)
			{
				model.CustomerId = Guid.Parse(userIdClaim.Value);
			}
			else
			{
				return Unauthorized();
			}

			var request = new UnlockSeatsCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}