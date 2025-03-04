using FB98.Shared.Abstractions.Entities;
using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Movies.Application.GenreManagement.GetAll
{
	public record GetAllGenreQuery(Filter Filter) : IQuery<ApiResult<PaginatedResult<GetAllGenreResponse>>>;
}