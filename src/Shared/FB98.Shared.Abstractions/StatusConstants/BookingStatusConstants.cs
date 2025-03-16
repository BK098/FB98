namespace FB98.Shared.Abstractions.StatusConstants
{
	public static class BookingStatusConstants
	{
		public static Guid Created = Guid.Parse("40a31c58-816e-4c6d-9bae-b0d250150f02");
		public static Guid Pending = Guid.Parse("b2a1a1e4-5d3b-4c6e-8a2e-1a2b3c4d5e6f");
		public static Guid Confirmed = Guid.Parse("d8ac0411-54ce-41af-afa2-949a9e605720");
		public static Guid CheckIn = Guid.Parse("10c6ca20-5e90-4c54-b62a-d56f1a433c39");
		public static Guid Abandoned = Guid.Parse("913e283f-293c-4803-9997-0493df83d908");
		public static Guid Cancelled = Guid.Parse("0492890c-b183-41cd-b318-592839ff50e0");
		public static Guid Expired = Guid.Parse("e3a1b2c3-d4e5-6f7a-8b9c-0d1e2f3a4b5c");

		public static string GetStatusName(Guid statusId)
		{
			return statusId switch
			{
				_ when statusId == Created => "Created",
				_ when statusId == Pending => "Pending",
				_ when statusId == CheckIn => "CheckIn",
				_ when statusId == Confirmed => "Confirmed",
				_ when statusId == Expired => "Expired",
				_ when statusId == Cancelled => "Cancelled",
				_ when statusId == Abandoned => "Abandoned",
				_ => "Unknown"
			};
		}
	}
}