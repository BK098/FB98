using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Customers.Api.Controllers
{
	[ApiController]
	[Route(BasePath + "/[controller]")]
	internal abstract class BaseController : ControllerBase
	{
		protected const string BasePath = "customer-module";
		protected readonly IMediator _mediator;

		protected BaseController(IMediator mediator)
		{
			_mediator = mediator;
		}
	}
}