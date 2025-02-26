using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.Application.DirectorManagement.GetAll
{
	internal sealed class GetAllDirectorQueryHandler : IQueryHandler<GetAllDirectorQuery, ApiResult<PaginatedResult<GetAllDirectorResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name"];
		private readonly IDirectorRepository _directorRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllDirectorQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllDirectorQueryHandler(
			IDirectorRepository directorRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllDirectorQueryHandler> logger,
			IMapper mapper)
		{
			_directorRepository = directorRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllDirectorResponse>>> Handle(GetAllDirectorQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var directors = _directorRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					directors = directors.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim()
						.Contains(search));
				}

				directors = directors.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);
				if (!await directors.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllDirectorResponse>>(
						_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var paginatedResult = await PaginatedResult<Director>.CreateAsync(
					directors,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllDirectorResponse>(
					_mapper.Map<List<GetAllDirectorResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all director");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllDirectorResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}