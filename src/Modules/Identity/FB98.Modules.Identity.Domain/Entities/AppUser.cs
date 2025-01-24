using Microsoft.AspNetCore.Identity;

namespace FB98.Modules.Identity.Domain.Entities
{
	public class AppUser : IdentityUser<Guid>
	{
		public string Firstname { get; set; } = string.Empty;
		public string Lastname { get; set; } = string.Empty;
		public byte Age { get; set; } = 0;
		public DateOnly BirthOfDate { get; set; }
		public virtual ICollection<TokenStore> TokenStores { get; set; } = new List<TokenStore>();
	}
}