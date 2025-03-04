namespace FB98.Modules.Movies.Application.CastManagement.Delete
{
	public record DeleteCastCommand(Guid CastId) : ICommand<ApiResult<object>>;
}