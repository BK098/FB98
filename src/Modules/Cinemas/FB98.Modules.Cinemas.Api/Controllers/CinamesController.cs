using FB98.Modules.Cinemas.Application.CinemaManagement.Create;
using FB98.Modules.Cinemas.Application.CinemaManagement.GetAll;
using FB98.Modules.Cinemas.Application.CinemaManagement.GetDetail;
using FB98.Modules.Cinemas.Application.CinemaManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Cinemas.Api.Controllers
{
	internal class CinamesController : BaseController
	{
		public CinamesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetAllCinema([FromQuery] Filter filter)
		{
			var request = new GetAllCinemaQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{cinemaId:guid}")]
		public async Task<IActionResult> GetDetailCinema(Guid cinemaId)
		{
			var request = new GetDetailCinemaQuery(cinemaId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		//[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateCinema([FromBody] CreateCinemaDto model)
		{
			var request = new CreateCinemaCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPut("{cinemaId:guid}")]
		public async Task<IActionResult> UpdateCinema(Guid cinemaId, [FromBody] UpdateCinemaDto model)
		{
			var request = new UpdateCinemaCommand(cinemaId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}