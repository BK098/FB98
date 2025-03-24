using FB98.Modules.Tickets.Application.BookingManagement.CheckIn;
using FB98.Modules.Tickets.Application.BookingManagement.GetAll;
using FB98.Modules.Tickets.Application.BookingManagement.GetDetail;
using FB98.Modules.Tickets.Application.BookingManagement.RetrieveShowSeat;
using FB98.Modules.Tickets.Application.BookingManagement.SeatReservation;
using FB98.Shared.Abstractions.Entities;
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

		[HttpGet]
		public async Task<IActionResult> GetAllBooking([FromQuery] Filter filter)
		{
			var request = new GetAllBookingQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
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
			var userEmailClaim = User.FindFirst(ClaimTypes.Email);
			var userPhoneClaim = User.FindFirst(ClaimTypes.MobilePhone);

			if (userIdClaim is null || userPhoneClaim is null || userEmailClaim is null)
			{
				return Unauthorized();
			}

			model.UserId = Guid.Parse(userIdClaim.Value);
			model.UserName = userEmailClaim.Value;
			model.UserPhone = userPhoneClaim.Value;

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
		[HttpPost("check-in")]
		public async Task<IActionResult> CheckIn([FromBody] CheckInDto model)
		{
			var request = new CheckInCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}