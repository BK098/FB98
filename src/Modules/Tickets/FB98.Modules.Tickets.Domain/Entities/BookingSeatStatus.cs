using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class BookingSeatStatus : BaseEntity
	{
		public string Name { get; set; }
	}

	public static class BookingSeatStatusConstants
	{
		public static Guid Available = Guid.Parse("0492890c-b183-41cd-b318-592839ff50e0");
		public static Guid Booked = Guid.Parse("bb2230e9-2869-4074-8e71-9645c4471102");
		public static Guid Pending = Guid.Parse("5e69d50b-60af-4367-b5d4-a3977a138bcb");
		public static Guid Cancelled = Guid.Parse("2e2d9591-b9e8-4475-b7fa-dcfeb3f4a8c9");
	}
}