using FB98.Modules.Tickets.Application.BookingManagement.Create;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Tickets.Api.Controllers
{
	internal class BookingsController : BaseController
	{
		public BookingsController(IMediator mediator) : base(mediator)
		{
		}

		//[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateBooking([FromBody] CreateBookingDto model)
		{
			var request = new CreateBookingCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}