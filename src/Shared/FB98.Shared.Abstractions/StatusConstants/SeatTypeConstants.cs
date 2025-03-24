namespace FB98.Shared.Abstractions.StatusConstants
{
	public static class SeatTypeConstants
	{
		public static Guid Normal { get; } = Guid.Parse("d9812abf-4348-4183-a227-25d2919a1097");
		public static Guid Couple { get; } = Guid.Parse("9bfce040-a42f-4bbb-9fa6-a30e8b9d7c53");
		public static Guid Vip { get; } = Guid.Parse("731c7e33-dfa1-48bb-a65a-1df30cd57346");

		public static string GetStatusName(Guid statusId)
		{
			return statusId switch
			{
				_ when statusId == Normal => "Normal",
				_ when statusId == Couple => "Couple",
				_ when statusId == Vip => "Vip",
				_ => "Unknown"
			};
		}
	}
}