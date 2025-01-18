using System.Threading.Channels;
using FB98.Shared.Abstractions.Messaging;

namespace FB98.Shared.Infrastructure.Messaging
{
	public interface IMessageChannel
	{
		ChannelReader<IMessage> Reader { get; }
		ChannelWriter<IMessage> Writer { get; }
	}
}