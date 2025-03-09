using FB98.Modules.Movies.Application.MovieManagement.Create;
using FB98.Modules.Movies.Application.MovieManagement.GetAll;
using FB98.Modules.Movies.Application.MovieManagement.GetDetail;
using FB98.Modules.Movies.Application.MovieManagement.Update;
using FB98.Shared.Abstractions.Entities;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Movies.Api.Controllers
{
	internal class MoviesController : BaseController
	{
		public MoviesController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public async Task<IActionResult> GetMovies([FromQuery] Filter filter)
		{
			var request = new GetAllMovieQuery(filter);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[HttpGet("{movieId:guid}")]
		public async Task<IActionResult> GetMovie(Guid movieId)
		{
			var request = new GetDetailMovieQuery(movieId);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPost]
		public async Task<IActionResult> CreateMovie([FromBody] CreateMovieDto model)
		{
			var request = new CreateMovieCommand(model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}

		[Authorize(Roles = "Administrator")]
		[HttpPut("{movieId:guid}")]
		public async Task<IActionResult> UpdateMovie(Guid movieId, [FromBody] UpdateMovieDto model)
		{
			var request = new UpdateMovieCommand(movieId, model);
			var result = await _mediator.Send(request);
			return StatusCode(result.StatusCode, result);
		}
	}
}