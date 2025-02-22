using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Identity.Api.Controllers
{
	[ApiController]
	[Route(BasePath + "/[controller]")]
	internal abstract class BaseController : ControllerBase
	{
		protected const string BasePath = "identity-module";
		protected readonly IMediator _mediator;
		protected BaseController(IMediator mediator)
		{
			_mediator = mediator;
		}
	}
}