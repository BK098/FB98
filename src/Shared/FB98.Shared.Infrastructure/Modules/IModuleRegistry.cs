using System.Collections.Generic;

namespace FB98.Shared.Infrastructure.Modules
{
	public interface IModuleRegistry
	{
		IEnumerable<ModuleBroadcastRegistration> GetBroadcastRegistration(string key);
		void AddBroadcastRegistration(ModuleBroadcastRegistration registration);
	}
}