using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Localization;
using System.Globalization;

namespace FB98.Shared.Infrastructure.Localization
{
	public class LocalizedMessageService : ILocalizedMessageService
	{
		private readonly IHttpContextAccessor _httpContextAccessor;

		public LocalizedMessageService(IHttpContextAccessor httpContextAccessor)
		{
			_httpContextAccessor = httpContextAccessor;
		}

		public string GetLocalizedMessage(string key, string? culture = null)
		{
			if (string.IsNullOrEmpty(culture))
			{
				var requestCulture = _httpContextAccessor.HttpContext?.Features.Get<IRequestCultureFeature>();
				culture = requestCulture?.RequestCulture.Culture.Name ?? "vi";
			}

			var currentCulture = new CultureInfo(culture);
			var message = Resources.Langauge.ResourceManager.GetString(key, currentCulture);

			return string.IsNullOrEmpty(message) ? $"[{key}] not found" : message;
		}
	}
}