using FB98.Shared.Abstractions.Events.Base;

namespace FB98.Modules.Customers.Domain.Events
{
	internal class CustomerCreatedEvent : IEvent
	{
		public Guid CustomerId { get; }
		public Guid UserId { get; }
		public string FullName { get; }

		public CustomerCreatedEvent(Guid customerId, Guid userId, string fullName)
		{
			CustomerId = customerId;
			UserId = userId;
			FullName = fullName;
		}
	}
}
