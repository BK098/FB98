namespace FB98.Shared.Abstractions.StatusConstants
{
	public static class OrderStatusConstants
	{
		public static readonly Guid Created = Guid.Parse("fc617b82-a90d-4d01-b9b9-2d4d59bcc9fd");
		public static readonly Guid Expired = Guid.Parse("fe9ddc54-0343-44d6-9a85-3068f7aaa267");
		public static readonly Guid Confirmed = Guid.Parse("0432df1a-e2e3-4b01-86a0-d15c4f54e85f");
		public static readonly Guid CheckedIn = Guid.Parse("a0c35adf-4c01-463e-a11b-98388254bbb5");
		public static readonly Guid Canceled = Guid.Parse("0177dc59-0f43-453d-9870-7ab8bcb539d3");

		public static string GetStatusName(Guid statusId)
		{
			return statusId switch
			{
				_ when statusId == Created => "Created",
				_ when statusId == Expired => "Expired",
				_ when statusId == Confirmed => "Confirmed",
				_ when statusId == Canceled => "Canceled",
				_ when statusId == CheckedIn => "CheckedIn",
				_ => "Unknown"
			};
		}
	}
}