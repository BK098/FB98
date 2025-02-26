namespace FB98.Modules.Movies.Application.DirectorManagement.Update
{
	public record UpdateDirectorCommand(Guid DirectorId, UpdateDirectorDto Model) : ICommand<ApiResult<object>>;
}