using Microsoft.AspNetCore.Mvc;

namespace FB98.Module.Systems.Api.Controllers
{
	[ApiController]
	[Route(BasePath + "/[controller]")]
	internal abstract class BaseController : ControllerBase
	{
		protected const string BasePath = "system-module";
		protected readonly IMediator _mediator;

		/// <inheritdoc />
		protected BaseController(IMediator mediator)
		{
			_mediator = mediator;
		}
	}
}