using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.DiscountManagement.GetAllDiscountRule
{
	internal sealed class GetAllDiscountRuleQueryHandler : IQueryHandler<GetAllDiscountRuleQuery, ApiResult<PaginatedResult<GetAllDiscountRuleResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name"];
		private readonly IProductDiscountRuleRepository _discountRuleRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllDiscountRuleQueryHandler> _logger;

		public GetAllDiscountRuleQueryHandler(
			IProductDiscountRuleRepository discountRuleRepository,
			ILogger<GetAllDiscountRuleQueryHandler> logger,
			ILocalizedMessageService localizedMessageService)
		{
			_discountRuleRepository = discountRuleRepository;
			_logger = logger;
			_localizedMessageService = localizedMessageService;
		}

		public async Task<ApiResult<PaginatedResult<GetAllDiscountRuleResponse>>> Handle(GetAllDiscountRuleQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			var today = DateTime.Today.ToUniversalTime();
			try
			{
				var entities = _discountRuleRepository.GetAll()
					.Include(x => x.Product)
					.Include(x => x.Combo)
					.Where(x => x.IsCombo == request.IsCombo);
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim().Contains(search));
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var grouped = await entities
					.AsNoTracking()
					.Where(x => request.IsCombo ? x.ComboId != null : x.ProductId != null) // lọc null trước khi group
					.GroupBy(x => request.IsCombo ? x.ComboId : x.ProductId)
					.ToListAsync(cancellationToken);


				var responses = grouped
					.Where(g => g.Key.HasValue && g.Any())
					.Select(g =>
					{
						var rules = g.ToList();

						var currentRule = rules
							.Where(r => r.IsValid())
							.OrderByDescending(r => r.StartDate)
							.FirstOrDefault();

						var firstItem = rules.First();

						return new GetAllDiscountRuleResponse
						{
							ProductId = g.Key ?? Guid.Empty,
							ProductName = request.IsCombo ? firstItem.Combo?.Name ?? string.Empty : firstItem.Product?.Name ?? string.Empty,
							ProductImage = request.IsCombo ? firstItem.Combo?.Image ?? string.Empty : firstItem.Product?.Image ?? string.Empty,
							CurrentRuleId = currentRule?.Id,
							CurrentRule = currentRule?.Name,
							UntilEnd = currentRule != null
								? (currentRule.EndDate - today).Days + " ngày"
								: "Không có",
							TotalRule = rules.Count
						};
					}).ToList();

				if (!responses.Any())
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllDiscountRuleResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = new PaginatedResult<GetAllDiscountRuleResponse>(
					responses,
					filter.PageIndex,
					filter.PageSize,
					grouped.Count);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all category");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllDiscountRuleResponse>>("An unexpected error occurred",
					500);
			}
		}
	}
}