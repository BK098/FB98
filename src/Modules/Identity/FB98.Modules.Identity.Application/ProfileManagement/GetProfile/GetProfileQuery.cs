namespace FB98.Modules.Identity.Application.ProfileManagement.GetProfile
{
	public record GetProfileQuery(GetProfileDto Model) : IQuery<ApiResult<GetProfileResponse>>;
}
