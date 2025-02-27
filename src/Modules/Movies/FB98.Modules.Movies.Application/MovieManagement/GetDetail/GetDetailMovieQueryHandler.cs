using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace FB98.Modules.Movies.Application.MovieManagement.GetDetail
{
	internal sealed class GetDetailMovieQueryHandler : IQueryHandler<GetDetailMovieQuery, ApiResult<GetDetailMovieResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailMovieQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMovieRepository _movieRepository;
		private readonly IConnectionMultiplexer _redisConnection;

		public GetDetailMovieQueryHandler(
			IMapper mapper,
			ILocalizedMessageService localizedMessageService,
			IMovieRepository movieRepository,
			ILogger<GetDetailMovieQueryHandler> logger,
			IConnectionMultiplexer redisConnection)
		{
			_mapper = mapper;
			_localizedMessageService = localizedMessageService;
			_movieRepository = movieRepository;
			_logger = logger;
			_redisConnection = redisConnection;
		}

		public async Task<ApiResult<GetDetailMovieResponse>> Handle(GetDetailMovieQuery request, CancellationToken cancellationToken)
		{
			var movieId = request.MovieId;
			var cacheKey = $"movie:{movieId}";
			IDatabase? redisDatabase = null;

			try
			{
				redisDatabase = _redisConnection.GetDatabase();
			}
			catch (RedisConnectionException ex)
			{
				_logger.LogWarning(ex, "Could not establish connection to Redis. Proceeding without cache.");
			}
			catch (RedisTimeoutException ex)
			{
				_logger.LogWarning(ex, "Redis timeout occurred. Skipping cache retrieval.");
			}

			try
			{
				if (redisDatabase != null)
				{
					var cachedMovie = await redisDatabase.StringGetAsync(cacheKey, CommandFlags.PreferMaster);
					if (!cachedMovie.IsNullOrEmpty)
					{
						var cachedResponse = JsonSerializer.Deserialize<GetDetailMovieResponse>(cachedMovie!);
						if (cachedResponse != null)
						{
							return ApiResponseBuilder.Success(cachedResponse, _localizedMessageService.GetLocalizedMessage("DataRetrievedFromCache"));
						}
					}
				}

				var movie = await _movieRepository.GetByIdAsync(movieId);
				if (movie is null)
				{
					return ApiResponseBuilder.Error<GetDetailMovieResponse>(_localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailMovieResponse>(movie);

				if (redisDatabase != null)
				{
					try
					{
						var random = new Random();
						var ramdomTime = TimeSpan.FromMinutes(random.Next(10, 20));
						await redisDatabase.StringSetAsync(cacheKey, JsonSerializer.Serialize(response), ramdomTime);
					}
					catch (RedisConnectionException ex)
					{
						_logger.LogWarning(ex, "Could not connect to Redis. Skipping cache save.");
					}
					catch (RedisTimeoutException ex)
					{
						_logger.LogWarning(ex, "Redis timeout occurred. Skipping cache save.");
					}
				}

				return ApiResponseBuilder.Success(response, _localizedMessageService.GetLocalizedMessage("DataRetrieved"));
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while get detail director");
				return ApiResponseBuilder.Error<GetDetailMovieResponse>("An unexpected error occurred", 500);
			}
		}
	}
}