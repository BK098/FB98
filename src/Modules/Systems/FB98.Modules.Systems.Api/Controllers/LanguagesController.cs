using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace FB98.Modules.Systems.Api.Controllers
{
	internal class LanguagesController : BaseController
	{
		[HttpPost("set-language")]
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