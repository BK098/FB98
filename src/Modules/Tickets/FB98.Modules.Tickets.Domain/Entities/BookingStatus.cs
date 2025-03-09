using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class BookingStatus : BaseEntity
	{
		public string Name { get; set; }
	}

	public static class BookingStatusConstants
	{
		public static Guid Pending = Guid.Parse("40a31c58-816e-4c6d-9bae-b0d250150f02");
		public static Guid Confirmed = Guid.Parse("d8ac0411-54ce-41af-afa2-949a9e605720");
		public static Guid CheckIn = Guid.Parse("10c6ca20-5e90-4c54-b62a-d56f1a433c39");
		public static Guid Abandoned = Guid.Parse("913e283f-293c-4803-9997-0493df83d908");
		public static Guid Canceled = Guid.Parse("0492890c-b183-41cd-b318-592839ff50e0");
	}
}