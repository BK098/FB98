using FB98.Shared.Infrastructure.Paging;

namespace FB98.Modules.Shows.Application.ShowManagement.GetAllByMovieId
{
	public record GetAllShowByMovieIdQuery(Guid MovieId) : IQuery<ApiResult<PaginatedResult<GetAllShowByMovieIdResponse>>>;
}