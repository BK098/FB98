using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Shows.Api.Controllers
{
	[Route(BasePath)]
	internal class HomeController : BaseController
	{
		public HomeController(IMediator mediator) : base(mediator)
		{
		}

		[HttpGet]
		public IActionResult Get()
		{
			return Ok("Show module");
		}
	}
}