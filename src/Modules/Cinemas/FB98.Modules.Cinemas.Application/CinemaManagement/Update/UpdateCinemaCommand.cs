namespace FB98.Modules.Cinemas.Application.CinemaManagement.Update
{
	public record UpdateCinemaCommand(Guid CinemaId, UpdateCinemaDto Model) : ICommand<ApiResult<object>>;
}
