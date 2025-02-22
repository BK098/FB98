using FB98.Shared.Abstractions.Events;
using MassTransit;

namespace FB98.Modules.Orders.Application.OrderManagement.Events
{
	public class PaymentSuccessEventHandler : IConsumer<PaymentSuccessEvent>
	{
		public Task Consume(ConsumeContext<PaymentSuccessEvent> context)
		{
			throw new NotImplementedException();
		}
	}
}
