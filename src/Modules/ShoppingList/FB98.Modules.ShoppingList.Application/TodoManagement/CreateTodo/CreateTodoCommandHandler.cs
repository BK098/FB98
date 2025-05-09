using AutoMapper;
using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Modules.ShoppingList.Domain.Entites;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodo
{
	internal sealed class CreateTodoCommandHandler : ICommandHandler<CreateTodoCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateTodoCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ITodoRepository _todoRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateTodoDto> _validator;

		public CreateTodoCommandHandler(
			IValidator<CreateTodoDto> validator,
			IMapper mapper,
			ITodoRepository todoRepository,
			ILogger<CreateTodoCommandHandler> logger,
			IUnitOfWork unitOfWork,
			ILocalizedMessageService localizedMessageService)
		{
			_validator = validator;
			_mapper = mapper;
			_todoRepository = todoRepository;
			_logger = logger;
			_unitOfWork = unitOfWork;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<object>> Handle(CreateTodoCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var todo = _mapper.Map<Todo>(model);
				await _todoRepository.CreateAsync(todo);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(todo.Id, _localizedMessageService.GetLocalizedMessage("Created"), 201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create todo");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}