namespace FB98.Modules.Movies.Application.DirectorManagement.Delete
{
	public record DeleteDirectorCommand(Guid DirectorId) : ICommand<ApiResult<object>>;
}