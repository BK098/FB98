using System.ComponentModel.DataAnnotations;

namespace FB98.Shared.Abstractions.Entities
{
	public abstract class BaseEntity : IEntity
	{
		[Key]
		public Guid Id { get; set; } = Guid.NewGuid();
		public DateTime CreateAt { get; private set; }
		public DateTime UpdateAt { get; private set; }

		public void SetUpdatedAt()
		{
			UpdateAt = DateTime.UtcNow;
		}

		public void SetCreatedAt()
		{
			CreateAt = DateTime.UtcNow;
		}
	}
}