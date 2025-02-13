using FB98.Shared.Abstractions.Messaging;
using System.Threading.Channels;

namespace FB98.Shared.Infrastructure.Messaging
{
	public interface IMessageChannel
	{
		ChannelReader<IMessage> Reader { get; }
		ChannelWriter<IMessage> Writer { get; }
	}
}