using FB98.Modules.Movies.Application.GenreManagement.GetAll;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Movies.Api.Controllers
{
	internal class GenresController : BaseController
	{
		public GenresController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetGenres([FromQuery] Filter filter)
		{
			var request = new GetAllGenreQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}