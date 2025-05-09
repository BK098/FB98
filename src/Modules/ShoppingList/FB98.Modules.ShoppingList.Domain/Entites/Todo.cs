using FB98.Shared.Abstractions.Entities;
using System.ComponentModel.DataAnnotations;

namespace FB98.Modules.ShoppingList.Domain.Entites
{
	public class Todo : BaseEntity
	{
		[StringLength(255)]
		public string Title { get; set; } = null!;
		public ICollection<TodoItem>? Items { get; set; } = new List<TodoItem>();
	}
}