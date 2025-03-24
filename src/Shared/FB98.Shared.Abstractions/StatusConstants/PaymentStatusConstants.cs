namespace FB98.Shared.Abstractions.StatusConstants
{
	public static class PaymentStatusConstants
	{
		public static Guid Pending { get; } = Guid.Parse("f2a17f10-3b1d-4887-80b3-6373dddeb70f");
		public static Guid Success { get; } = Guid.Parse("9ff67eb7-f88f-402e-ba66-aec9160bfae5");
		public static Guid Failed { get; } = Guid.Parse("6b0996b4-0f5d-41fe-8077-eda605d75f60");
	}
}