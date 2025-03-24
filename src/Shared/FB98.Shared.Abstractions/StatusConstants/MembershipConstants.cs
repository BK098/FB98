namespace FB98.Shared.Abstractions.StatusConstants
{
	public static class MembershipConstants
	{
		/// <summary>
		///     Rank đầu tiên của người dùng khi thanh toán
		/// </summary>
		public static Guid Silver { get; } = Guid.Parse("d2a4f8b1-3c4e-4b8e-9b8d-1f2a3b4c5d6e");
		/// <summary>
		///     Rank thứ hai của người dùng khi thanh toán
		/// </summary>
		public static Guid Gold { get; } = Guid.Parse("e3b5c9d2-4d5f-5e9f-8c7d-2f3b4c5d6e7f");
		/// <summary>
		///     Rank thứ ba của người dùng khi thanh toán
		/// </summary>
		public static Guid Platinum { get; } = Guid.Parse("f4c6d0e3-5e6f-6f0f-9d8e-3f4b5c6d7e8f");
		/// <summary>
		///     Rank cuối cùng của người dùng khi thanh toán
		/// </summary>
		public static Guid Diamond { get; } = Guid.Parse("05d7e1f4-6f7f-7f1f-0e9f-4f5b6c7d8e9f");

		public static string GetStatusName(Guid statusId)
		{
			return statusId switch
			{
				_ when statusId == Silver => "Silver",
				_ when statusId == Gold => "Gold",
				_ when statusId == Platinum => "Platinum",
				_ when statusId == Diamond => "Diamond",
				_ => "Unknown"
			};
		}

		public static Guid GetStatusId(string statusName)
		{
			return statusName switch
			{
				"Silver" => Silver,
				"Gold" => Gold,
				"Platinum" => Platinum,
				"Diamond" => Diamond,
				_ => throw new ArgumentOutOfRangeException(nameof(statusName), statusName, null)
			};
		}
	}
}