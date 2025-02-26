namespace FB98.Modules.Movies.Application.CastManagement.Update
{
	public record UpdateCastCommand(Guid CastId, UpdateCastDto Model) : ICommand<ApiResult<object>>;
}