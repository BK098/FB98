using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Shows.Domain.Entities
{
	public class ShowStatus : BaseEntity
	{
		[StringLength(20)]
		public string Name { get; init; } = null!;
	}
}