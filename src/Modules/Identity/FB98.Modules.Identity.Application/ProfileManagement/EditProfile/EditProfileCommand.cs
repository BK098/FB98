namespace FB98.Modules.Identity.Application.ProfileManagement.EditProfile
{
	public record EditProfileCommand(Guid UserId, EditProfileDto Model) : ICommand<ApiResult<object>>;
}
