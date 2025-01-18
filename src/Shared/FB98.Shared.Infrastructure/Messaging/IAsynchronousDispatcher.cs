using System.Threading.Tasks;
using FB98.Shared.Abstractions.Messaging;

namespace FB98.Shared.Infrastructure.Messaging
{
	public interface IAsynchronousDispatcher
	{
		Task PublishAsync<TMessage>(TMessage message) where TMessage : class, IMessage;
	}
}