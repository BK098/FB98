namespace FB98.Modules.Movies.Application.MovieManagement.Create
{
	public record CreateMovieCommand(CreateMovieDto Model) : ICommand<ApiResult<object>>;
}
