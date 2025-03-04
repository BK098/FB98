using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.Application.CastManagement.GetAll
{
	internal sealed class GetAllCastQueryHandler : IQueryHandler<GetAllCastQuery, ApiResult<PaginatedResult<GetAllCastReponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name"];
		private readonly ICastRepository _castRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllCastQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllCastQueryHandler(
			ILogger<GetAllCastQueryHandler> logger,
			ICastRepository castRepository,
			ILocalizedMessageService localizedMessageService,
			IMapper mapper)
		{
			_logger = logger;
			_castRepository = castRepository;
			_localizedMessageService = localizedMessageService;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllCastReponse>>> Handle(GetAllCastQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var casts = _castRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					casts = casts.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim()
						.Contains(search));
				}

				if (!await casts.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllCastReponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				casts = casts.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<Cast>.CreateAsync(
					casts,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllCastReponse>(
					_mapper.Map<List<GetAllCastReponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all cast");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllCastReponse>>("An unexpected error occurred", 500);
			}
		}
	}
}