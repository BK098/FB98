using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Movies.Application.MovieManagement.GetAll
{
	public record GetAllMovieQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllMovieResponse>>>;
}