using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Movies.Application.DirectorManagement.GetAll
{
	public record GetAllDirectorQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllDirectorResponse>>>;
}