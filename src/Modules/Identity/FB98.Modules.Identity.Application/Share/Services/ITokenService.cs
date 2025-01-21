using FB98.Modules.Identity.Application.Share.Entities;

namespace FB98.Modules.Identity.Application.Share.Services
{
	public interface ITokenService
	{
		string GenerateJwtToken(AppUser user);
		string GenerateRefreshToken();
	}
}