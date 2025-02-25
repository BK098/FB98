using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Cinemas.Domain.Entities
{
	public class SeatType : BaseEntity
	{
		public string Name { get; set; }
	}

	public static class SeatTypeConstants
	{
		public static Guid Normal = Guid.Parse("d9812abf-4348-4183-a227-25d2919a1097");
		public static Guid Couple = Guid.Parse("9bfce040-a42f-4bbb-9fa6-a30e8b9d7c53");
		public static Guid Vip = Guid.Parse("731c7e33-dfa1-48bb-a65a-1df30cd57346");
	}
}