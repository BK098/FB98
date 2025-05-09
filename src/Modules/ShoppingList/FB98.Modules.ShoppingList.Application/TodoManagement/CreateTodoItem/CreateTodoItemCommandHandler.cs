using AutoMapper;
using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Modules.ShoppingList.Domain.Entites;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Refits;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FluentValidation;
using Microsoft.Extensions.Logging;
using Refit;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.CreateTodoItem
{
	internal sealed class CreateTodoItemCommandHandler : ICommandHandler<CreateTodoItemCommand, ApiResult<object>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<CreateTodoItemCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ITodoRepository _todoRepository;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<CreateTodoItemDto> _validator;
		private readonly ICatalogApi _catalogApi;
		public CreateTodoItemCommandHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<CreateTodoItemCommandHandler> logger,
			IMapper mapper,
			ITodoRepository todoRepository,
			IUnitOfWork unitOfWork,
			IValidator<CreateTodoItemDto> validator,
			ICatalogApi catalogApi)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_todoRepository = todoRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_catalogApi = catalogApi;
		}

		public async Task<ApiResult<object>> Handle(CreateTodoItemCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
			try
			{
				var valiationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!valiationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(valiationResult.Errors);
				}

				var todo = await _todoRepository.GetByIdAsync(model.TodoId);
				if (todo == null)
				{
					return ApiResponseBuilder.Error<object>(
						"Todo " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var todoItem = _mapper.Map<TodoItem>(model);

				if (model.ProductId != null)
				{
					try
					{
						var product = await _catalogApi.GetProductById(model.ProductId!.Value);
						todoItem.ProductId = product.Data!.Id;
						todoItem.Name = product.Data.Name;
						todoItem.Unit = "Unknow At This Time";
					}
					catch (ApiException e)
					{
						_logger.LogInformation(e, "Error occurred while get product");
					}
				}

				_todoRepository.CreateTodoItem(todoItem);
				await _unitOfWork.SaveChangesAsync();
				return ApiResponseBuilder.Success<object>(todo.Id, _localizedMessageService.GetLocalizedMessage("Created"),
					201);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while create todo item");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}
	}
}