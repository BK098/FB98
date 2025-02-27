namespace FB98.Modules.Movies.Application.MovieManagement.GetDetail
{
	public record GetDetailMovieQuery(Guid MovieId) : IQuery<ApiResult<GetDetailMovieResponse>>;
}