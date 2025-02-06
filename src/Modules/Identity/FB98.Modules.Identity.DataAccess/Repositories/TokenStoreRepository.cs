using FB98.Modules.Identity.Application.Abtractions;
using FB98.Modules.Identity.DataAccess.Data;
using FB98.Modules.Identity.Domain.Entities;
using Microsoft.EntityFrameworkCore;
namespace FB98.Modules.Identity.DataAccess.Repositories
{
	public class TokenStoreRepository : ITokenStoreRepository
	{
		private readonly IdentityModuleDbContext _context;

		public TokenStoreRepository(IdentityModuleDbContext context)
		{
			_context = context;
		}

		public async Task<TokenStore?> GetByTokenAsync(string token)
		{
			return await _context.RefreshTokens
				.FirstOrDefaultAsync(t => t.Token == token);
		}

		public async Task<List<TokenStore>> GetByUserIdAsync(Guid userId)
		{
			return await _context.RefreshTokens
				.Where(t => t.UserId == userId && !t.IsRevoked)
				.ToListAsync();
		}

		public async Task AddAsync(TokenStore refreshToken)
		{
			await _context.RefreshTokens.AddAsync(refreshToken);
			await _context.SaveChangesAsync();
		}

		public async Task UpdateAsync(TokenStore refreshToken)
		{
			_context.RefreshTokens.Update(refreshToken);
			await _context.SaveChangesAsync();
		}

		public async Task RevokeAllByUserIdAsync(Guid userId)
		{
			var tokens = await _context.RefreshTokens
				.Where(t => t.UserId == userId && !t.IsRevoked)
				.ToListAsync();

			foreach (var token in tokens)
			{
				token.IsRevoked = true;
			}

			await _context.SaveChangesAsync();
		}

		public async Task RevokeByDeviceIdAsync(Guid userId, Guid deviceId)
		{
			var tokens = await _context.RefreshTokens
				.Where(t => t.UserId == userId && t.DeviceId == deviceId && !t.IsRevoked)
				.ToListAsync();

			foreach (var token in tokens)
			{
				token.IsRevoked = true;
			}

			await _context.SaveChangesAsync();
		}

		public async Task<TokenStore?> GetByDeviceIdAsync(Guid deviceId, Guid userId)
		{
			return await _context.RefreshTokens
				.FirstOrDefaultAsync(t => t.DeviceId == deviceId && t.UserId == userId);
		}
	}

}
