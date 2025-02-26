using FB98.Modules.Identity.Domain.Entities;

namespace FB98.Modules.Identity.Application.Services
{
	public interface ITokenService
	{
		Task<string> GenerateAccessToken(AppUser user);
		string GenerateRefreshToken();
	}
}