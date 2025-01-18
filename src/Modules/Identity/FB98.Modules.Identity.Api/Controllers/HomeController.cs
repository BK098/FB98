using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Identity.Api.Controllers
{
	[Route(BasePath)]
	internal class HomeController : BaseController
	{
		[HttpGet]
		public IActionResult Get()
		{
			return Ok("Identity module");
		}
	}
}