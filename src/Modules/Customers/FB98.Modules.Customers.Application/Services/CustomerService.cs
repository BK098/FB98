using FB98.Shared.Abstractions.Events.Base;

namespace FB98.Modules.Customers.Application.Services
{
	public class CustomerService
	{
		private readonly IEventDispatcher _eventDispatcher;
		public CustomerService(IEventDispatcher eventDispatcher)
		{
			_eventDispatcher = eventDispatcher;
		}


	}
}
