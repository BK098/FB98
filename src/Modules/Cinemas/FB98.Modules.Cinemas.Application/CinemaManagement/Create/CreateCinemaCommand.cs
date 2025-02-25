namespace FB98.Modules.Cinemas.Application.CinemaManagement.Create
{
	public record CreateCinemaCommand(CreateCinemaDto Model) : ICommand<ApiResult<object>>;
}