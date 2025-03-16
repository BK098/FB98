using FB98.Modules.Tickets.Application.BookingManagement.CheckIn;
using FB98.Modules.Tickets.Application.BookingManagement.GetDetail;
using FB98.Modules.Tickets.Application.BookingManagement.RetrieveShowSeat;
using FB98.Modules.Tickets.Application.BookingManagement.SeatReservation;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;
using System.Security.Claims;

namespace FB98.Modules.Tickets.Api.Controllers
{
	internal class BookingsController : BaseController
	{
		public BookingsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet("{bookingId:guid}")]
		public async Task<IActionResult> GetBooking(Guid bookingId)
		{
			var request = new GetDetailBookingQuery(bookingId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize]
		[HttpPost("seat-reservation")]
		public async Task<IActionResult> SeatReservation([FromBody] SeatReservationDto model)
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

			var request = new SeatReservationCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{showId:guid}/seats")]
		public async Task<IActionResult> RetrieveShowSeat(Guid showId)
		{
			var request = new RetrieveShowSeatQuery(showId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("{bookingId:guid}check-in")]
		public async Task<IActionResult> CheckIn(Guid bookingId)
		{
			var request = new CheckInCommand(bookingId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}