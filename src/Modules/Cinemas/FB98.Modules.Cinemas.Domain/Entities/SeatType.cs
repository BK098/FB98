using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Cinemas.Domain.Entities
{
	public class SeatType : BaseEntity
	{
		[StringLength(50)]
		public string Name { get; init; } = null!;
	}
}