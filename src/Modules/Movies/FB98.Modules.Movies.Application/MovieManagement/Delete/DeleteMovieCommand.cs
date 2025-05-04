namespace FB98.Modules.Movies.Application.MovieManagement.Delete
{
	public record DeleteMovieCommand(Guid MovieId) : ICommand<ApiResult<object>>;
}