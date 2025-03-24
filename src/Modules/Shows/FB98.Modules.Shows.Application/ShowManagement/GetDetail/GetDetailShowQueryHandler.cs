using AutoMapper;
using FB98.Modules.Shows.Application.Abstractions;
using StackExchange.Redis;
using System.Text.Json;

namespace FB98.Modules.Shows.Application.ShowManagement.GetDetail
{
	internal sealed class GetDetailShowQueryHandler : IQueryHandler<GetDetailShowQuery, ApiResult<GetDetailShowResponse>>
	{
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<GetDetailShowQueryHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IConnectionMultiplexer _redisConnection;
		private readonly IShowRepository _showRepository;

		public GetDetailShowQueryHandler(
			ILocalizedMessageService localizedMessageService,
			ILogger<GetDetailShowQueryHandler> logger,
			IMapper mapper,
			IShowRepository showRepository,
			IConnectionMultiplexer redisConnection)
		{
			_localizedMessageService = localizedMessageService;
			_logger = logger;
			_mapper = mapper;
			_showRepository = showRepository;
			_redisConnection = redisConnection;
		}

		public async Task<ApiResult<GetDetailShowResponse>> Handle(GetDetailShowQuery request, CancellationToken cancellationToken)
		{
			var showId = request.ShowId;
			var cacheKey = $"show:{showId}";
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
					var cachedMovie = await redisDatabase.StringGetAsync(cacheKey);
					if (!cachedMovie.IsNullOrEmpty)
					{
						var cachedResponse = JsonSerializer.Deserialize<GetDetailShowResponse>(cachedMovie!);
						if (cachedResponse != null)
						{
							return ApiResponseBuilder.Success(cachedResponse, _localizedMessageService.GetLocalizedMessage("DataRetrievedFromCache"));
						}
					}
				}

				var show = await _showRepository.GetByIdAsync(showId);
				if (show == null)
				{
					return ApiResponseBuilder.Error<GetDetailShowResponse>("Show: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var response = _mapper.Map<GetDetailShowResponse>(show);
				if (redisDatabase != null)
				{
					try
					{
						var random = new Random();
						var ramdomTime = TimeSpan.FromMinutes(random.Next(1, 5));
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
				_logger.LogError(ex, "Error occurred while create new discount rule");
				return ApiResponseBuilder.Error<GetDetailShowResponse>("An unexpected error occurred", 500);
			}
		}
	}
}