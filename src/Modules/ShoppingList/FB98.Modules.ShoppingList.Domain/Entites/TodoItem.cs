using FB98.Shared.Abstractions.Entities;

namespace FB98.Modules.ShoppingList.Domain.Entites
{
	public class TodoItem : BaseEntity
	{
		public string? Note { get; set; }
		public Guid TodoId { get; set; }
		public Todo? Todo { get; set; }

		public Guid? ParentItemId { get; set; }
		public TodoItem? ParentItem { get; set; }
		public ICollection<TodoItem>? SubItems { get; set; }

		public Guid? ProductId { get; set; }
		public string? Unit { get; set; } // from Product
		public int? Quantity { get; set; } // from Product
		public string? Name { get; set; } // from Product
		public decimal? PriceAtTime { get; set; }
		public bool IsChecked { get; set; } = false;
	}
}