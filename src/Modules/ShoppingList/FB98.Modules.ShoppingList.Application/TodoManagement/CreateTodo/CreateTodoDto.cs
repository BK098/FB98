namespace FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodo
{
	public class CreateTodoDto
	{
		public string Title { get; set; } = null!;
		//public ICollection<CreateTodoItemDto>? Items { get; set; }
	}

	//public class CreateTodoItemDto
	//{
	//	public string? Note { get; set; }
	//	public Guid ProductId { get; set; }
	//	public string? Unit { get; set; }
	//	public string? Name { get; set; }
	//	public decimal PriceAtTime { get; set; }
	//}
}