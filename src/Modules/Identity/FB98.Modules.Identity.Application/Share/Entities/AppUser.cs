using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Application.Share.Entities
{
	public class AppUser : IdentityUser<Guid>
	{
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public byte Age { get; set; } = 0;
		// Xử lý RefreshToken
		public string? RefreshToken { get; set; }
		public DateTime RefreshTokenExpiryTime { get; set; }
		// Xử Lý Revocation (Thu Hồi Refresh Token)
		public bool IsRevoked { get; set; } = false;
		public DateTime? RevokedAt { get; set; }
	}
}