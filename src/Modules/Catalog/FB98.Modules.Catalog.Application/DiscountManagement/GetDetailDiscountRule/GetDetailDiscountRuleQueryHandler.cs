using FB98.Modules.Catalog.Application.Abstractions;
using FB98.Modules.Catalog.Application.DiscountManagement.GetAllDiscountRule;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Catalog.Application.DiscountManagement.GetDetailDiscountRule
{
	internal sealed class GetDetailDiscountRuleQueryHandler : IQueryHandler<GetDetailDiscountRuleQuery, ApiResult<PaginatedResult<GetDetailDiscountRuleResponse>>>
	{
		private readonly IProductDiscountRuleRepository _discountRuleRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllDiscountRuleQueryHandler> _logger;

		public GetDetailDiscountRuleQueryHandler(
			IProductDiscountRuleRepository discountRuleRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllDiscountRuleQueryHandler> logger)
		{
			_discountRuleRepository = discountRuleRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
		}

		public async Task<ApiResult<PaginatedResult<GetDetailDiscountRuleResponse>>> Handle(GetDetailDiscountRuleQuery request, CancellationToken cancellationToken)
		{
			try
			{
				var query = _discountRuleRepository.GetAll()?
					.Where(x => x.IsCombo == request.IsCombo &&
								(request.IsCombo ? x.ComboId == request.ProductId : x.ProductId == request.ProductId))
					.OrderByDescending(x => x.StartDate);

				if (query == null)
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetDetailDiscountRuleResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var total = await query.CountAsync(cancellationToken);

				var items = await query
					.Select(x => new GetDetailDiscountRuleResponse
					{
						Id = x.Id,
						Name = x.Name,
						Description = x.Description,
						Value = x.Value,
						IsDiscountPercentage = x.IsDiscountPercentage,
						StartDate = x.StartDate.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz"),
						EndDate = x.EndDate.ConvertUtcToVietnamTime().ToString("dd-MM-yyyy HH:mm:ss zz")
					})
					.ToListAsync(cancellationToken);

				var result = new PaginatedResult<GetDetailDiscountRuleResponse>(
					items,
					1,
					total,
					total
				);

				return ApiResponseBuilder.Success(result, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while getting detail discount rules");
				return ApiResponseBuilder.Error<PaginatedResult<GetDetailDiscountRuleResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}