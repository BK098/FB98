namespace FB98.Shared.Abstractions.Events.Base
{
	public interface IEventHandler<in TEvent> where TEvent : class, IEvent
	{
		Task HandleAsync(TEvent @event);
	}
}