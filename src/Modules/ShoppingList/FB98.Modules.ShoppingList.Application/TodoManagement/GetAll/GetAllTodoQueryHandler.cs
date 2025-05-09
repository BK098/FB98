using AutoMapper;
using FB98.Modules.ShoppingList.Application.Abstractions;
using FB98.Modules.ShoppingList.Domain.Entites;
using FB98.Shared.Abstractions.CQRS;
using FB98.Shared.Abstractions.Responses;
using FB98.Shared.Infrastructure.Localization;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;
using Microsoft.Extensions.Logging;

namespace FB98.Modules.ShoppingList.Application.TodoManagement.GetAll
{
	internal sealed class
		GetAllTodoQueryHandler : IQueryHandler<GetAllTodoQuery, ApiResult<PaginatedResult<GetAllTodoResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Title"];
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllTodoQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly ITodoRepository _todoRepository;

		public GetAllTodoQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllTodoQueryHandler> logger,
			IMapper mapper,
			ITodoRepository todoRepository)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_todoRepository = todoRepository;
		}

		public async Task<ApiResult<PaginatedResult<GetAllTodoResponse>>> Handle(
			GetAllTodoQuery request,
			CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var entities = _todoRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x => EF.Functions.Unaccent(x.Title).ToLower().Trim().Contains(search));
				}

				if (!await entities.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllTodoResponse>>(
						_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<Todo>.CreateAsync(
					entities, filter.PageIndex, filter.PageSize, cancellationToken);

				var response = new PaginatedResult<GetAllTodoResponse>(
					_mapper.Map<List<GetAllTodoResponse>>(paginatedResult.Items), paginatedResult.PageIndex,
					paginatedResult.PageSize, paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all todo");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllTodoResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}