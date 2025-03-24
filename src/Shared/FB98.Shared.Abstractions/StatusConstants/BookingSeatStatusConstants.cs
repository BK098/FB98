namespace FB98.Shared.Abstractions.StatusConstants
{
	public static class BookingSeatStatusConstants
	{
		public static Guid Available { get; } = Guid.Parse("0492890c-b183-41cd-b318-592839ff50e0");
		public static Guid Pending { get; } = Guid.Parse("5e69d50b-60af-4367-b5d4-a3977a138bcb");
		public static Guid Booked { get; } = Guid.Parse("2e2d9591-b9e8-4475-b7fa-dcfeb3f4a8c9");
		public static Guid CheckIn { get; } = Guid.Parse("6c26d077-1908-4fb1-ad5d-b4679ae66e6a");

		public static string GetStatusName(Guid statusId)
		{
			return statusId switch
			{
				_ when statusId == Available => "Available",
				_ when statusId == Pending => "Pending",
				_ when statusId == Booked => "Booked",
				_ when statusId == CheckIn => "CheckIn",
				_ => "Unknown"
			};
		}
	}
}