using FB98.Modules.Movies.Application.MovieManagement.Create;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Movies.Api.Controllers
{
	internal class MoviesController : BaseController
	{
		public MoviesController(IMediator mediator) : base(mediator)
		{
		}


		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateMovie([FromForm] CreateMovieDto model)
		{
			model.Deserialize();
			var request = new CreateMovieCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}