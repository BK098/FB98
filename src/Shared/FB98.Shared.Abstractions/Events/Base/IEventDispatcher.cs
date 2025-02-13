namespace FB98.Shared.Abstractions.Events.Base
{
	public interface IEventDispatcher
	{
		Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;
	}
}