using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Module.Systems.Api.Controllers
{
	internal class LanguageController : BaseController
	{
		public LanguageController(IMediator mediator) : base(mediator)
		{
		}

		[HttpPost("{culture}")]
		public IActionResult SetLanguage(string culture)
		{
			var supportedCultures = new[] { "en", "vi" };

			if (!Array.Exists(supportedCultures, c => c == culture))
			{
				return BadRequest("Culture not supported");
			}

			var cookieValue = CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture));
			Response.Cookies.Append(CookieRequestCultureProvider.DefaultCookieName, cookieValue);

			return Ok(new { message = "Language changed", culture });
		}
	}
}
