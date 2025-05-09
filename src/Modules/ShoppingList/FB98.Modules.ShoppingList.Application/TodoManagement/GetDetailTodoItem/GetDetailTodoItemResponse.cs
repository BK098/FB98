namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetDetailTodoItem
{
	public class GetDetailTodoItemResponse
	{
		public Guid Id { get; set; }
		public string? Note { get; set; }
		public Guid? ProductId { get; set; }
		public string? Unit { get; set; }
		public string? Name { get; set; }
		public int? Quantity { get; set; }
		public decimal? PriceAtTime { get; set; }
		public bool IsChecked { get; set; }
		public List<GetDetailTodoItemResponse>? SubItems { get; set; }
	}
}
