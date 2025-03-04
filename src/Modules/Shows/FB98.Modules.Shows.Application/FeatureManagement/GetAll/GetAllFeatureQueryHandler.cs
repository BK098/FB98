using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.Application.FeatureManagement.GetAll
{
	internal sealed class GetAllFeatureQueryHandler : IQueryHandler<GetAllFeatureQuery, ApiResult<PaginatedResult<GetAllFeatureResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name"];
		private readonly IFeatureRepository _featureRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllFeatureQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllFeatureQueryHandler(
			IFeatureRepository featureRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllFeatureQueryHandler> logger,
			IMapper mapper)
		{
			_featureRepository = featureRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllFeatureResponse>>> Handle(GetAllFeatureQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var entities = _featureRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim()
						.Contains(search));
				}

				if (!await entities.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllFeatureResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<Feature>.CreateAsync(
					entities,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllFeatureResponse>(
					_mapper.Map<List<GetAllFeatureResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all features");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllFeatureResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}