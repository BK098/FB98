namespace FB98.Modules.Movies.Application.MovieManagement.Update
{
	public record UpdateMovieCommand(Guid MovieId, UpdateMovieDto Model) : ICommand<ApiResult<object>>;
}