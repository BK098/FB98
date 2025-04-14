using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Tickets.Domain.Entities
{
	public class BookingSeatStatus : BaseEntity
	{
		[StringLength(50)]
		public string Name { get; init; } = null!;
	}
}