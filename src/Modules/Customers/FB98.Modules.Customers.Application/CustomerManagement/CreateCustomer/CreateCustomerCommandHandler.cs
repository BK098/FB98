using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Customers.Application.CustomerManagement.CreateCustomer
{
	internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, ApiResult<object>>
	{
		public Task<ApiResult<object>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
