using System.Globalization;

namespace FB98.Shared.Infrastructure.Localization
{
	public class LocalizedMessageService : ILocalizedMessageService
	{
		public string GetLocalizedMessage(string key, string culture = "vi")
		{
			var currentCulture = new CultureInfo(culture);
			var message = Resources.Langauge.ResourceManager.GetString(key, currentCulture);

			if (string.IsNullOrEmpty(message))
			{
				return $"[{key}] not found";
			}
			return message;
		}
	}
}
