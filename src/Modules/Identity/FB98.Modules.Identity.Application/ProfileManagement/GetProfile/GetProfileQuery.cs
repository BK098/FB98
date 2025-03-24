namespace FB98.Modules.Identity.Application.ProfileManagement.GetProfile
{
	public record GetProfileQuery(string UserId) : IQuery<ApiResult<GetProfileResponse>>;
}
