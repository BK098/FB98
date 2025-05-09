using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Modules.ShoppingList.Application.TodoManagement.GetDetailTodoItem;
using FB98.Modules.ShoppingList.Domain.Entites;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetDetail
{
	internal sealed class GetDetailTodoQueryHandler : IQueryHandler<GetDetailTodoQuery, ApiResult<GetDetailTodoResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailTodoQueryHandler> _logger;
		private readonly ITodoRepository _todoRepository;

		public GetDetailTodoQueryHandler(
			ITodoRepository todoRepository,
			ILogger<GetDetailTodoQueryHandler> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_todoRepository = todoRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<GetDetailTodoResponse>> Handle(
			GetDetailTodoQuery request,
			CancellationToken cancellationToken)
		{
			var todoId = request.TodoId;
			try
			{
				var todo = await _todoRepository.GetByIdAsync(todoId);
				if (todo == null)
				{
					return ApiResponseBuilder.Error<GetDetailTodoResponse>(
						_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var allItems = todo.Items?.ToList() ?? new List<TodoItem>();

				var itemDict = allItems.ToDictionary(i => i.Id, MapItem);
				foreach (var item in allItems)
				{
					if (item.ParentItemId.HasValue && itemDict.ContainsKey(item.ParentItemId.Value))
					{
						itemDict[item.ParentItemId.Value].SubItems ??= new List<GetDetailTodoItemResponse>();
						itemDict[item.ParentItemId.Value].SubItems?.Add(itemDict[item.Id]);
					}
				}

				var rootItems = allItems.Where(i => i.ParentItemId == null).Select(i => itemDict[i.Id]).ToList();

				var response = new GetDetailTodoResponse
				{
					Title = todo.Title,
					Items = rootItems
				};
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail todo");
				return ApiResponseBuilder.Error<GetDetailTodoResponse>("An unexpected error occurred", 500);
			}
		}

		private static GetDetailTodoItemResponse MapItem(TodoItem item)
		{
			return new GetDetailTodoItemResponse
			{
				Id = item.Id,
				Note = item.Note,
				ProductId = item.ProductId,
				Unit = item.Unit,
				Name = item.Name,
				Quantity = item.Quantity,
				PriceAtTime = item.PriceAtTime,
				IsChecked = item.IsChecked,
				SubItems = []
			};
		}
	}
}