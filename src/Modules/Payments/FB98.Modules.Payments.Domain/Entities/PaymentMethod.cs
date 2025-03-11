using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class PaymentMethod : BaseEntity
	{
		public string Name { get; set; }
		public ICollection<PaymentTransaction> PaymentTrannsactions { get; set; } = new List<PaymentTransaction>();
	}
}
