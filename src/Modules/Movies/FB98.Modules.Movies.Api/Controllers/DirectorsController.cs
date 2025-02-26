using FB98.Modules.Movies.Application.DirectorManagement.Create;
using FB98.Modules.Movies.Application.DirectorManagement.GetAll;
using FB98.Modules.Movies.Application.DirectorManagement.GetDetail;
using FB98.Modules.Movies.Application.DirectorManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Movies.Api.Controllers
{
	internal class DirectorsController : BaseController
	{
		public DirectorsController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetDirectors([FromQuery] Filter filter)
		{
			var request = new GetAllDirectorQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{directorId:guid}")]
		public async Task<IActionResult> GetDirector(Guid directorId)
		{
			var request = new GetDetailDirectorQuery(directorId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateDirector([FromBody] CreateDirectorDto model)
		{
			var request = new CreateDirectorCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{directorId:guid}")]
		public async Task<IActionResult> UpdateDirector(Guid directorId, [FromBody] UpdateDirectorDto model)
		{
			var request = new UpdateDirectorCommand(directorId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		//[Authorize(Roles = "Administrator")]
		//[HttpDelete("{directorId:guid}")]
		//public async Task<IActionResult> DeleteDirector(Guid directorId)
		//{
		//	var request = new DeleteDirectorCommand(directorId);
		//	var result = await _mediator.Send(request);
		//	return StatusCode(result.StatusCode, result);
		//}
	}
}