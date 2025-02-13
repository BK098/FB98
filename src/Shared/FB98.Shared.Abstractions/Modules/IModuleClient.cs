namespace FB98.Shared.Abstractions.Modules
{
	public interface IModuleClient
	{
		Task PublishAsync(object message);
	}
}