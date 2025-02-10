using System.ComponentModel.DataAnnotations;

namespace FB98.Shared.Abstractions.Entities
{
	public abstract class BaseEntity : IEntity
	{
		[Key]
		public Guid Id { get; set; }
		public DateTime CreateAt { get; private set; } = DateTime.Now;
		public DateTime UpdateAt { get; set; }
	}
}
