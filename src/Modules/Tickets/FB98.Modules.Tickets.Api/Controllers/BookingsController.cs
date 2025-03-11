using FB98.Modules.Tickets.Application.BookingManagement.SeatReservation;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Tickets.Api.Controllers
{
	internal class BookingsController : BaseController
	{
		public BookingsController(IMediator mediator) : base(mediator)
		{
		}

		//[Authorize(Roles = "Administrator")]
		[HttpPost("seat-reservation")]
		public async Task<IActionResult> SeatReservation([FromBody] SeatReservationDto model)
		{
			var request = new SeatReservationCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}