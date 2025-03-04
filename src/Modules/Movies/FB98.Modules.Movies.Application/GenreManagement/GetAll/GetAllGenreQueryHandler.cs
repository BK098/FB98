using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.Application.GenreManagement.GetAll
{
	internal sealed class GetAllGenreQueryHandler : IQueryHandler<GetAllGenreQuery, ApiResult<PaginatedResult<GetAllGenreResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Name"];
		private readonly IGenreRepository _genreRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetAllGenreQueryHandler> _logger;
		private readonly IMapper _mapper;

		public GetAllGenreQueryHandler(
			IGenreRepository genreRepository,
			ILocalizedMessageService localizedMessageService,
			ILogger<GetAllGenreQueryHandler> logger,
			IMapper mapper)
		{
			_genreRepository = genreRepository;
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
		}

		public async Task<ApiResult<PaginatedResult<GetAllGenreResponse>>> Handle(GetAllGenreQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var genres = _genreRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					genres = genres.Where(x => EF.Functions.Unaccent(x.Name).ToLower().Trim()
						.Contains(search));
				}

				if (!await genres.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllGenreResponse>>(
						_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				genres = genres.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);

				var paginatedResult = await PaginatedResult<Genre>.CreateAsync(
					genres,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllGenreResponse>(
					_mapper.Map<List<GetAllGenreResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all genre");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllGenreResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}