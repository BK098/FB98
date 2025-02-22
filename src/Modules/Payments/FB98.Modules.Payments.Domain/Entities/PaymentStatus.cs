using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class PaymentStatus : BaseEntity
	{
		public string Name { get; set; }
		public ICollection<PaymentTransaction> PaymentTrannsactions { get; set; } = new List<PaymentTransaction>();
	}

	public static class PaymentStatusConstants
	{
		public static readonly Guid Peding = Guid.Parse("f2a17f10-3b1d-4887-80b3-6373dddeb70f");
		public static readonly Guid Success = Guid.Parse("9ff67eb7-f88f-402e-ba66-aec9160bfae5");
		public static readonly Guid Failed = Guid.Parse("6b0996b4-0f5d-41fe-8077-eda605d75f60");
	}
}