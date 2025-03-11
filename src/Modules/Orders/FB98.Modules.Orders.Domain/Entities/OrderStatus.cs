using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.Orders.Domain.Entities
{
	public class OrderStatus : BaseEntity
	{
		public string Name { get; init; }
	}
}