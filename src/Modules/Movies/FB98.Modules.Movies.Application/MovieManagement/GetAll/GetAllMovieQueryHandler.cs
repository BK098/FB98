using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Application.MovieManagement.GetDetail;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Paging;
using FB98.Shared.Utils.Extensions;
using Microsoft.EntityFrameworkCore;

namespace FB98.Modules.Movies.Application.MovieManagement.GetAll
{
	internal sealed class GetAllMovieQueryHandler : IQueryHandler<GetAllMovieQuery, ApiResult<PaginatedResult<GetAllMovieResponse>>>
	{
		private readonly List<string> _allowedProperties = ["Title"];
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailMovieQueryHandler> _logger;
		private readonly IMapper _mapper;

		private readonly IMovieRepository _movieRepository;

		//private readonly IConnectionMultiplexer _redisConnection;
		public GetAllMovieQueryHandler(ILocalizedMessageService localizedMessageService, ILogger<GetDetailMovieQueryHandler> logger, IMapper mapper, IMovieRepository movieRepository)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_movieRepository = movieRepository;
		}

		public async Task<ApiResult<PaginatedResult<GetAllMovieResponse>>> Handle(GetAllMovieQuery request, CancellationToken cancellationToken)
		{
			var filter = request.Filter;
			try
			{
				var movies = _movieRepository.GetAll();
				var search = filter.SearchTerm?.ConvertToUnsign().Trim();
				if (!string.IsNullOrEmpty(search))
				{
					movies = movies.Where(x => EF.Functions.Unaccent(x.Title).ToLower().Trim()
						.Contains(search));
				}

				if (!await movies.AnyAsync(cancellationToken))
				{
					return ApiResponseBuilder.Error<PaginatedResult<GetAllMovieResponse>>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				movies = movies.SortBy(filter.SortColumn, _allowedProperties, filter.IsDescending);
				movies = movies
					.Include(x => x.Genres).ThenInclude(x => x.Genre);
				var paginatedResult = await PaginatedResult<Movie>.CreateAsync(
					movies,
					filter.PageIndex,
					filter.PageSize,
					cancellationToken);

				var response = new PaginatedResult<GetAllMovieResponse>(
					_mapper.Map<List<GetAllMovieResponse>>(paginatedResult.Items),
					paginatedResult.PageIndex,
					paginatedResult.PageSize,
					paginatedResult.TotalCount);

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get all movie");
				return ApiResponseBuilder.Error<PaginatedResult<GetAllMovieResponse>>("An unexpected error occurred", 500);
			}
		}
	}
}