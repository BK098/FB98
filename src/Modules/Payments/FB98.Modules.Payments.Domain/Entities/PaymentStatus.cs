using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Payments.Domain.Entities
{
	public class PaymentStatus : BaseEntity
	{
		[StringLength(255)]
		public string Name { get; init; } = null!;
		public ICollection<PaymentTransaction> PaymentTrannsactions { get; set; } = new List<PaymentTransaction>();
	}
}