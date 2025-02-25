using AutoMapper;
using FB98.Modules.Cinemas.Application.Abstractions;
using FB98.Modules.Cinemas.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Cinemas.Application.CinemaManagement.GetAll
{
	internal sealed class GetAllCinemaQueryHandler : IQueryHandler<GetAllCinemaQuery, ApiResult<PaginatedResult<GetAllCinemaResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name, Address"];
		private readonly ICinemaRepository _cinemaRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllCinemaQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllCinemaQueryHandler(
			ICinemaRepository cinemaRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllCinemaQueryHandler> logger,
			IMapper mapper)
		{
			_cinemaRepository = cinemaRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllCinemaResponse>>> Handle(GetAllCinemaQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var cinemas = _cinemaRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					cinemas = cinemas.Where(x =>
						EF.Functions.Unaccent(x.Name).ToLower().Trim().Contains(search) ||
						EF.Functions.Unaccent(x.Address).ToLower().Trim().Contains(search));
				}

				cinemas = cinemas.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);
				if (!await cinemas.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllCinemaResponse>>(
						_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var paginatedResult = await PaginatedResult<Cinema>.CreateAsync(
					cinemas,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);
				var cinemaForView = _mapper.Map<List<GetAllCinemaResponse>>(paginatedResult.Items);

				var paginatedCinemaForView = new PaginatedResult<GetAllCinemaResponse>(
					cinemaForView,
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(paginatedCinemaForView);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all cinemas");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllCinemaResponse>>("An unexpected error occurred",
					500);
			}
		}
	}
}