using System.Threading.Tasks;

namespace FB98.Shared.Abstractions.Events
{
	public interface IEventDispatcher
	{
		Task PublishAsync<TEvent>(TEvent @event) where TEvent : class, IEvent;
	}
}