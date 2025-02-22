using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class PaymentMethod : BaseEntity
	{
		public string Name { get; set; }
		public ICollection<PaymentTransaction> PaymentTrannsactions { get; set; } = new List<PaymentTransaction>();
	}

	public static class PaymentMethodConstants
	{
		public static readonly Guid VnPayCard = Guid.Parse("a86c5ba1-426a-46b6-a3a6-9b6d61520f2f");
		public static readonly Guid Cash = Guid.Parse("16316f65-e37a-4210-8033-0aa39d19403e");
		public static readonly Guid VnPayQrCode = Guid.Parse("d273a4e2-438f-4dc3-8bd6-81846882b2b9");
	}
}
