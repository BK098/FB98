using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;

namespace FB98.Modules.Customers.Application.CustomerManagement.GetDetail
{
	public record GetDetailCustomerQuery(Guid UserId) : IQuery<ApiResult<GetDetailCustomerResponse>>;
}