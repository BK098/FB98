namespace FB98.Modules.Movies.Application.CastManagement.Create
{
	public record CreateCastCommand(CreateCastDto Model) : ICommand<ApiResult<object>>;
}