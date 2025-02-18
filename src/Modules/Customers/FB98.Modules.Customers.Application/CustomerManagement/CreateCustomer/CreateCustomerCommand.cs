using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Customers.Application.CustomerManagement.CreateCustomer
{
	public record CreateCustomerCommand(Guid UserId, string FullName) : ICommand<ApiResult<object>>;
}
