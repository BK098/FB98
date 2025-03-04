using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Shows.Application.FeatureManagement.GetAll
{
	public record GetAllFeatureQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllFeatureResponse>>>;
}