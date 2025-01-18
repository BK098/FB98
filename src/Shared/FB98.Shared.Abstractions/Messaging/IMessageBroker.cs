using System.Threading.Tasks;

namespace FB98.Shared.Abstractions.Messaging
{
	public interface IMessageBroker
	{
		Task PublishAsync(params IMessage[] messages);
	}
}