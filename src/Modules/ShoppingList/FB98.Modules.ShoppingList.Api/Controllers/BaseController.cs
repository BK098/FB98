using MediatR;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.ShoppingList.Api.Controllers
{
	[ApiController]
	[Route(BasePath + "/[controller]")]
	internal abstract class BaseController : ControllerBase
	{
		protected const string BasePath = "shoppingList-module";
		protected readonly IMediator _mediator;

		protected BaseController(IMediator mediator)
		{
			_mediator = mediator;
		}
	}
}