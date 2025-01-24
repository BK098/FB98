using FB98.Modules.Identity.Domain.Entities;

namespace FB98.Modules.Identity.Application.Abtractions
{
	public interface ITokenStoreRepository
	{
		Task<TokenStore?> GetByTokenAsync(string token);
		Task<List<TokenStore>> GetByUserIdAsync(Guid userId);
		Task AddAsync(TokenStore refreshToken);
		Task UpdateAsync(TokenStore refreshToken);
		Task RevokeAllByUserIdAsync(Guid userId);
		Task RevokeByDeviceIdAsync(Guid userId, Guid deviceId);
		Task<TokenStore?> GetByDeviceIdAsync(Guid deviceId, Guid userId);
	}
}