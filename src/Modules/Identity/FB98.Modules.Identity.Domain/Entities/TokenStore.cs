using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Identity.Domain.Entities
{
	public class TokenStore
	{
		public Guid Id { get; set; }
		public string Token { get; set; } = string.Empty;
		public Guid? DeviceId { get; set; }
		public string? DeviceName { get; set; } = string.Empty;
		public string IpAddress { get; set; } = string.Empty;
		public string UserAgent { get; set; } = string.Empty;
		public DateTime CreatedAt { get; set; }
		public DateTime ExpiresAt { get; set; }
		public bool IsRevoked { get; set; } = false;

		[ForeignKey("AppUser")]
		public Guid UserId { get; set; }
		public AppUser AppUser { get; set; } = default!;
	}
}