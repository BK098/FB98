using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using FB98.Modules.Shows.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Shows.Application.FeatureTypeManagement.GetAll
{
	internal sealed class GetAllFeatureTypeQueryHandler : IQueryHandler<GetAllFeatureTypeQuery, ApiResult<PaginatedResult<GetAllFeatureTypeResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name"];
		private readonly IFeatureTypeRepository _featureTypeRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllFeatureTypeQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllFeatureTypeQueryHandler(
			IFeatureTypeRepository featureTypeRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllFeatureTypeQueryHandler> logger,
			IMapper mapper)
		{
			_featureTypeRepository = featureTypeRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllFeatureTypeResponse>>> Handle(GetAllFeatureTypeQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var entities = _featureTypeRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					entities = entities.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim().Contains(search));
				}

				if (!await entities.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllFeatureTypeResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				entities = entities.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<FeatureType>.CreateAsync(
					entities,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllFeatureTypeResponse>(
					_mapper.Map<List<GetAllFeatureTypeResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all featureTypes");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllFeatureTypeResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}