using FB98.Modules.Movies.Application.CastManagement.Create;
using FB98.Modules.Movies.Application.CastManagement.GetAll;
using FB98.Modules.Movies.Application.CastManagement.GetDetail;
using FB98.Modules.Movies.Application.CastManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Movies.Api.Controllers
{
	internal class CastsController : BaseController
	{
		public CastsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetCasts([FromQuery] Filter filter)
		{
			var request = new GetAllCastQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{castId:guid}")]
		public async Task<IActionResult> GetCast(Guid castId)
		{
			var request = new GetDetailCastQuery(castId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateCast([FromBody] CreateCastDto model)
		{
			var request = new CreateCastCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{castId:guid}")]
		public async Task<IActionResult> UpdateCast(Guid castId, [FromBody] UpdateCastDto model)
		{
			var request = new UpdateCastCommand(castId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		//[Authorize(Roles = "adminstrator")]
		//[HttpDelete("{castId:guid}")]
		//public async Task<IActionResult> DeleteCast(Guid castId)
		//{
		//	var request = new DeleteCastCommand(castId);
		//	var result = await _mediator.Send(request);
		//	return StatusCode(result.StatusCode, result);
		//}
	}
}
