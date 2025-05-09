using FB98.Modules.ShoppingList.Application.TodoManagement.GetDetailTodoItem;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetDetail
{
	public class GetDetailTodoResponse
	{
		public string Title { get; set; } = null!;
		public IEnumerable<GetDetailTodoItemResponse>? Items { get; set; }
	}
}