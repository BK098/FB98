using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Identity.Api.Controllers
{
	[ApiController]
	[Route(BasePath + "/[controller]")]
	internal abstract class BaseController : ControllerBase
	{
		protected const string BasePath = "identity-module";
	}
}