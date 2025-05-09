using AutoMapper;
using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodoItem
{
	internal sealed class UpdateTodoItemCommandHandler : ICommandHandler<UpdateTodoItemCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateTodoItemCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ITodoRepository _todoRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateTodoItemDto> _validator;

		public UpdateTodoItemCommandHandler(
			ITodoRepository todoRepository,
			IUnitOfWork unitOfWork,
			IMapper mapper,
			IValidator<UpdateTodoItemDto> validator,
			ILocalizedMessageService localizedMessageService,
			ILogger<UpdateTodoItemCommandHandler> logger)
		{
			_todoRepository = todoRepository;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_validator = validator;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
		}

		public async Task<ApiResult<object>> Handle(UpdateTodoItemCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var todoItemId = request.TodoItemId;
			try
			{
				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var item = await _todoRepository.GetTodoItemByIdAsync(todoItemId);
				if (item == null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("ItemNotFound"), 404);
				}

				_mapper.Map(model, item);
				_todoRepository.UpdateTodoItem(item);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(todoItemId, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating Todo item");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}