using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Payments.Application.PaymentManagement.GetPaymentHisotry
{
	public record GetPaymentHisotryQuery(Guid UserId, Filter Filter) : IQuery<ApiResult<PaginatedResult<GetPaymentHisotryResponse>>>;
}