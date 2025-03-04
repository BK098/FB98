using FB98.Modules.Shows.Application.ShowManagement.Create;
using FB98.Modules.Shows.Application.ShowManagement.CreateRange;
using FB98.Modules.Shows.Application.ShowManagement.Delete;
using FB98.Modules.Shows.Application.ShowManagement.GetAll;
using FB98.Modules.Shows.Application.ShowManagement.GetAllByMovieId;
using FB98.Modules.Shows.Application.ShowManagement.GetDetail;
using FB98.Modules.Shows.Application.ShowManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Shows.Api.Controllers
{
	internal class ShowsController : BaseController
	{
		public ShowsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet("{movieId:guid}/movie")]
		public async Task<IActionResult> GetAllShow(Guid movieId)
		{
			var request = new GetAllShowByMovieIdQuery(movieId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet]
		public async Task<IActionResult> GetAllShow([FromQuery] Filter filter)
		{
			var request = new GetAllShowQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{showId:guid}")]
		public async Task<IActionResult> GetDetailShow(Guid showId)
		{
			var request = new GetDetailShowQuery(showId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("create-range")]
		public async Task<IActionResult> CreateRangeShow([FromBody] CreateRangeShowDto model)
		{
			var request = new CreateRangeShowCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost("create")]
		public async Task<IActionResult> CreateShow([FromBody] CreateShowDto model)
		{
			var request = new CreateShowCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{showId:guid}")]
		public async Task<IActionResult> UpdateShow(Guid showId, [FromBody] UpdateShowDto model)
		{
			var request = new UpdateShowCommand(showId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpDelete("{showId:guid}")]
		public async Task<IActionResult> DeleteShow(Guid showId)
		{
			var request = new DeleteShowCommand(showId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}