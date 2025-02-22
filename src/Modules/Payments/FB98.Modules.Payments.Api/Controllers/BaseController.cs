using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Payments.Api.Controllers
{
	[ApiController]
	[Route(BasePath + "/[controller]")]
	internal abstract class BaseController : ControllerBase
	{
		protected const string BasePath = "payment-module";
		protected readonly IMediator _mediator;

		protected BaseController(IMediator mediator)
		{
			_mediator = mediator;
		}
	}
}