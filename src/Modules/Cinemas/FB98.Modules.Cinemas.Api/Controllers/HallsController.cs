using FB98.Modules.Cinemas.Application.HallManagement.CheckSeats;
using FB98.Modules.Cinemas.Application.HallManagement.Create;
using FB98.Modules.Cinemas.Application.HallManagement.GetDetail;
using FB98.Modules.Cinemas.Application.HallManagement.Update;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Cinemas.Api.Controllers
{
	internal class HallsController : BaseController
	{
		public HallsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet("{hallId:guid}/seats")]
		public async Task<IActionResult> GetDetailHallWithSeats(Guid hallId)
		{
			var request = new GetDetailHallQuery(hallId, true);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{hallId:guid}")]
		public async Task<IActionResult> GetDetailHall(Guid hallId)
		{
			var request = new GetDetailHallQuery(hallId, false);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost]
		public async Task<IActionResult> CreateHall([FromBody] CreateHallDto model)
		{
			var request = new CreateHallCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPost("{hallId:guid}/check-seats")]
		public async Task<IActionResult> CreateHall(Guid hallId, [FromBody] CheckSeatsDto model)
		{
			var request = new CheckSeatsCommand(hallId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpPut("{hallId:guid}")]
		public async Task<IActionResult> UpdateHall(Guid hallId, [FromBody] UpdateHallDto model)
		{
			var request = new UpdateHallCommand(hallId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}