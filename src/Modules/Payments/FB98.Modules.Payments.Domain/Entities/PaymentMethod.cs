using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class PaymentMethod : BaseEntity
	{
		[StringLength(255)]
		public string Name { get; set; } = null!;
		public ICollection<PaymentTransaction> PaymentTrannsactions { get; set; } = new List<PaymentTransaction>();
	}
}
