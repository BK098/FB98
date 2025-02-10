using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Customers.Application.CustomerManagement.CreateCustomer
{
	internal sealed class CreateCustomerCommandHandler : ICommandHandler<CreateCustomerCommand, ApiResponse<object>>
	{
		public Task<ApiResponse<object>> Handle(CreateCustomerCommand request, CancellationToken cancellationToken)
		{
			throw new NotImplementedException();
		}
	}
}
