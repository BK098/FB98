using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Orders.Domain.Entities
{
	public class OrderStatus : BaseEntity
	{
		[StringLength(50)]
		public string Name { get; init; } = null!;
	}
}