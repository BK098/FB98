namespace FB98.Shared.Infrastructure.Localization
{
	public interface ILocalizedMessageService
	{
		string GetLocalizedMessage(string key, string culture = "vi");
	}
}
