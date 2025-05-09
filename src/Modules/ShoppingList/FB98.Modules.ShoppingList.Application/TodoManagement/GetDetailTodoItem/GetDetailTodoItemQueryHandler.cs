using AutoMapper;
using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetDetailTodoItem
{
	internal sealed class GetDetailTodoItemQueryHandler : IQueryHandler<GetDetailTodoItemQuery, ApiResult<GetDetailTodoItemResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailTodoItemQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ITodoRepository _todoRepository;
		public GetDetailTodoItemQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetDetailTodoItemQueryHandler> logger,
			IMapper mapper,
			ITodoRepository todoRepository)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_todoRepository = todoRepository;
		}
		public async Task<ApiResult<GetDetailTodoItemResponse>> Handle(
			GetDetailTodoItemQuery request,
			CancellationToken cancellationToken)
		{
			var todoItemId = request.TodoItemId;
			try
			{
				var todoItem = await _todoRepository.GetTodoItemByIdAsync(todoItemId);
				if (todoItem == null)
				{
					return ApiResponseBuilder.Error<GetDetailTodoItemResponse>(
						_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailTodoItemResponse>(todoItem);
				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail category");
				return ApiResponseBuilder.Error<GetDetailTodoItemResponse>("An unexpected error occurred", 500);
			}
		}
	}
}