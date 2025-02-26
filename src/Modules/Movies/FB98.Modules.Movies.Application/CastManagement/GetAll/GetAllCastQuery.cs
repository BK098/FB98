using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Movies.Application.CastManagement.GetAll
{
	public record GetAllCastQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllCastReponse>>>;
}