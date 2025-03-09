using FB98.Modules.Tickets.Application.SeatManagement.LockSeat;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Tickets.Api.Controllers
{
	internal class LockSeatsController : BaseController
	{
		public LockSeatsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost]
		public async Task<IActionResult> LockSeats([FromBody] LockSeatsDto model)
		{
			var request = new LockSeatsCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}