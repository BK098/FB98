using FB98.Shared.Abstractions.Responses;
using Refit;
using System.Runtime.CompilerServices;

[assembly: InternalsVisibleTo("FB98.Modules.Payments.Application")]
namespace FB98.Shared.Abstractions.Refits
{
	public  interface ICustomerApi
	{
		[Get("/customer-module/Customer?userId={userId}")]
		Task<ApiResult<CustomerDto>> GetCustomerById(Guid userId);
	}

	public record CustomerDto(Guid UserId, Guid MembershipId, int MembershipDiscount);
}