using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Shows.Application.FeatureTypeManagement.GetAll
{
	public record GetAllFeatureTypeQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllFeatureTypeResponse>>>;
}