using System.ComponentModel.DataAnnotations.Schema;

namespace FB98.Modules.Identity.Domain.Entities
{
	public class TokenStore
	{
		public Guid Id { get; set; }
		public string Token { get; set; } = null!;
		public Guid? DeviceId { get; set; }
		public string? DeviceName { get; set; }
		public string IpAddress { get; set; } = null!;
		public string UserAgent { get; set; } = null!;
		public DateTime CreatedAt { get; set; }
		public DateTime ExpiresAt { get; set; }
		public bool IsRevoked { get; set; } = false;

		[ForeignKey("AppUser")]
		public Guid UserId { get; set; }
		public AppUser? AppUser { get; set; }
	}
}