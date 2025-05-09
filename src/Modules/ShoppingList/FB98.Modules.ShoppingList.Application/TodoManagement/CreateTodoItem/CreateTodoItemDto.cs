namespace FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodoItem
{
	public class CreateTodoItemDto
	{
		public Guid TodoId { get; set; }
		public Guid? TodoItemId { get; set; }
		public string? Note { get; set; }
		public Guid? ProductId { get; set; }
		public int Quantity { get; set; }
		public string? Unit { get; set; }
		public string? Name { get; set; }
		public decimal? PriceAtTime { get; set; }
	}
}