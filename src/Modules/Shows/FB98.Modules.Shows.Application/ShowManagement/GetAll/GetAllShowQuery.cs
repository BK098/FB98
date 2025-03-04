using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Shows.Application.ShowManagement.GetAll
{
	public record GetAllShowQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllShowResponse>>>;
}