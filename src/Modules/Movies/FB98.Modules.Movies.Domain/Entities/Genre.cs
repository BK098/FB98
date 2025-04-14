using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.Movies.Domain.Entities
{
	public class Genre : BaseEntity
	{
		[StringLength(255)]
		public string Name { get; set; } = null!;
		public string? Description { get; set; }
	}
}