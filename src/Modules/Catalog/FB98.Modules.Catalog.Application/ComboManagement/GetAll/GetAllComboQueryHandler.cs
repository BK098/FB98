using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.ComboManagement.GetAll
{
	internal sealed class GetAllComboQueryHandler : IQueryHandler<GetAllComboQuery, ApiResult<PaginatedResult<GetAllComboResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name", "Price"];
		private readonly IComboRepository _comboRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllComboQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllComboQueryHandler(
			ILogger<GetAllComboQueryHandler> logger,
			IComboRepository comboRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper)
		{
			_logger = logger;
			_comboRepository = comboRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllComboResponse>>> Handle(GetAllComboQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var combos = _comboRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					combos = combos.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim().Contains(search));
				}

				if (!await combos.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllComboResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				combos = combos.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<Combo>.CreateAsync(
					combos,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllComboResponse>(
					_mapper.Map<List<GetAllComboResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all combos");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllComboResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}