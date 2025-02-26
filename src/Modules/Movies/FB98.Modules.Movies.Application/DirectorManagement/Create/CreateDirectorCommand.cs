namespace FB98.Modules.Movies.Application.DirectorManagement.Create
{
	public record CreateDirectorCommand(CreateDirectorDto Model) : ICommand<ApiResult<object>>;
}