using AutoMapper;
using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.UpdateTodo
{
	internal class UpdateTodoCommandHandler : ICommandHandler<UpdateTodoCommand, ApiResult<object>>
	{
		private readonly ITodoRepository _todoRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateTodoCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateTodoDto> _validator;

		public UpdateTodoCommandHandler(
			ILogger<UpdateTodoCommandHandler> logger,
			ITodoRepository todoRepository,
			IUnitOfWork unitOfWork,
			IMapper mapper,
			IValidator<UpdateTodoDto> validator,
			ILocalizedMessageService localizedMessage)
		{
			_logger = logger;
			_todoRepository = todoRepository;
			_unitOfWork = unitOfWork;
			_mapper = mapper;
			_validator = validator;
			_localizedMessageService = localizedMessage;
		}

		public async Task<ApiResult<object>> Handle(UpdateTodoCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			var todoId = request.TodoId;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var todo = await _todoRepository.GetByIdAsync(todoId);
				if (todo is null)
				{
					return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				if (todo.Title != model.Title)
				{
					var todoExisted = await _todoRepository.IsTodoExistsAsync(model.Title!, cancellationToken);
					if (todoExisted)
					{
						return ApiResponseBuilder.Error<object>(_localizedMessageService.GetLocalizedMessage("Existed"));
					}
				}

				_mapper.Map(model, todo);
				_todoRepository.Update(todo);
				await _unitOfWork.SaveChangesAsync();

				return ApiResponseBuilder.Success<object>(todoId, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while update todo");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}
