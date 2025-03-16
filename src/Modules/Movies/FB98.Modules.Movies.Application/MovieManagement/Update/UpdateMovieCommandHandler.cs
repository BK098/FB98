using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Domain.Entities;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace FB98.Modules.Movies.Application.MovieManagement.Update
{
	internal sealed class UpdateMovieCommandHandler : ICommandHandler<UpdateMovieCommand, ApiResult<object>>
	{
		private readonly ICastRepository _castRepository;
		private readonly IDirectorRepository _directorRepository;
		private readonly IGenreRepository _genreRepository;
		private readonly ILocalizedMessageService _localizedMessageService;
		private readonly ILogger<UpdateMovieCommandHandler> _logger;
		private readonly IMapper _mapper;
		private readonly IMovieRepository _movieRepository;
		private readonly IConnectionMultiplexer _redisConnection;
		private readonly IUnitOfWork _unitOfWork;
		private readonly IValidator<UpdateMovieDto> _validator;

		public UpdateMovieCommandHandler(
			IMapper mapper,
			ILogger<UpdateMovieCommandHandler> logger,
			IMovieRepository movieRepository,
			ICastRepository castRepository,
			IDirectorRepository directorRepository,
			IGenreRepository genreRepository,
			IUnitOfWork unitOfWork,
			IValidator<UpdateMovieDto> validator,
			ILocalizedMessageService localizedMessageService,
			IConnectionMultiplexer redisConnection)
		{
			_mapper = mapper;
			_logger = logger;
			_movieRepository = movieRepository;
			_castRepository = castRepository;
			_directorRepository = directorRepository;
			_unitOfWork = unitOfWork;
			_validator = validator;
			_localizedMessageService = localizedMessageService;
			_redisConnection = redisConnection;
			_genreRepository = genreRepository;
		}

		public async Task<ApiResult<object>> Handle(UpdateMovieCommand request, CancellationToken cancellationToken)
		{
			var model = request.Model;
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
				var movie = await _movieRepository.GetByIdAsync(movieId);
				if (movie == null)
				{
					return ApiResponseBuilder.Error<object>("Movie: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var validationResult = await _validator.ValidateAsync(model, cancellationToken);
				if (!validationResult.IsValid)
				{
					return ApiResponseBuilder.ValidationError<object>(validationResult.Errors);
				}

				var castIds = model.Casts.Select(c => c.Id).ToHashSet();
				var existingCasts = (await _castRepository.GetAll()
					.Where(g => castIds.Contains(g.Id))
					.ToListAsync(cancellationToken)).ToDictionary(g => g.Id);
				if (existingCasts.Count != castIds.Count)
				{
					return ApiResponseBuilder.Error<object>("CastIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var directorIds = model.Directors.Select(d => d.Id).ToHashSet();
				var existingDirectors = (await _directorRepository.GetAll()
					.Where(d => directorIds.Contains(d.Id))
					.ToListAsync(cancellationToken)).ToDictionary(g => g.Id);
				if (existingDirectors.Count != directorIds.Count)
				{
					return ApiResponseBuilder.Error<object>("DirectorIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var genreIds = model.Genres.Select(d => d.Id).ToHashSet();
				var existinggenres = (await _genreRepository.GetAll()
					.Where(d => genreIds.Contains(d.Id))
					.ToListAsync(cancellationToken)).ToDictionary(g => g.Id);
				if (existinggenres.Count != genreIds.Count)
				{
					return ApiResponseBuilder.Error<object>("GenreIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_mapper.Map(model, movie);
				await UpdateGenres(movie, model.Genres);
				await UpdateDirectors(movie, model.Directors);
				await UpdateCasts(movie, model.Casts);

				_unitOfWork.Entry(movie, EntityState.Modified);
				await _unitOfWork.SaveChangesAsync();

				try
				{
					if (redisDatabase == null)
					{
						return ApiResponseBuilder.Success<object>(movie.Id, _localizedMessageService.GetLocalizedMessage("Updated"));
					}

					await redisDatabase.KeyDeleteAsync(cacheKey);
				}
				catch (RedisConnectionException ex)
				{
					_logger.LogWarning(ex, "Could not connect to Redis. Skipping cache save.");
				}
				catch (RedisTimeoutException ex)
				{
					_logger.LogWarning(ex, "Redis timeout occurred. Skipping cache save.");
				}

				return ApiResponseBuilder.Success<object>(movie.Id, _localizedMessageService.GetLocalizedMessage("Updated"));
			}
			catch (DbUpdateConcurrencyException ex)
			{
				_logger.LogError(ex, "Error occurred while updating movie");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
			catch (Exception ex)
			{
				_logger.LogError(ex, "Error occurred while updating movie");
				return ApiResponseBuilder.Error<object>("An unexpected error occurred", 500);
			}
		}

		private async Task UpdateGenres(Movie movie, ICollection<UpdateMovieGenreDto> genreDtos)
		{
			var newGenreIds = genreDtos.Select(g => g.Id!.Value).ToList();

			var genresToRemove = movie.Genres.Where(g => !newGenreIds.Contains(g.GenreId)).ToList();
			foreach (var genre in genresToRemove)
			{
				_unitOfWork.Entry(genre, EntityState.Deleted);
			}

			var allGenres = await _genreRepository.GetByIdsAsync(newGenreIds);

			foreach (var genreDto in genreDtos)
			{
				var genreId = genreDto.Id!.Value;
				var existingGenre = movie.Genres.FirstOrDefault(x => x.GenreId == genreId);

				var genre = allGenres.FirstOrDefault(cm => cm.Id == genreId);
				if (existingGenre != null)
				{
					_unitOfWork.Entry(existingGenre, EntityState.Modified);
				}
				else
				{
					var newMovieGenre = new MovieGenre
					{
						GenreId = genre!.Id,
						MovieId = movie.Id
					};
					_unitOfWork.Entry(newMovieGenre, EntityState.Added);
				}
			}
		}

		private async Task UpdateCasts(Movie movie, ICollection<UpdateMovieCastDto> castDtos)
		{
			var newCastIds = castDtos.Select(c => c.Id!.Value).ToList();

			var castToRemove = movie.Casts.Where(c => !newCastIds.Contains(c.CastId)).ToList();
			foreach (var cast in castToRemove)
			{
				_unitOfWork.Entry(cast, EntityState.Deleted);
			}

			var allCasts = await _castRepository.GetByIdsAsync(newCastIds);

			foreach (var castDto in castDtos)
			{
				var castId = castDto.Id!.Value;
				var existingGenre = movie.Genres.FirstOrDefault(x => x.GenreId == castId);

				var cast = allCasts.FirstOrDefault(cm => cm.Id == castId);
				if (existingGenre != null)
				{
					_unitOfWork.Entry(existingGenre, EntityState.Modified);
				}
				else
				{
					var newCast = new MovieCast
					{
						CastId = cast!.Id,
						MovieId = movie.Id
					};
					_unitOfWork.Entry(newCast, EntityState.Added);
				}
			}
		}

		private async Task UpdateDirectors(Movie movie, ICollection<UpdateMovieDirectorDto> castDtos)
		{
			var newDirectorIds = castDtos.Select(c => c.Id!.Value).ToList();

			var directorToRemove = movie.Directors.Where(c => !newDirectorIds.Contains(c.DirectorId)).ToList();
			foreach (var director in directorToRemove)
			{
				_unitOfWork.Entry(director, EntityState.Deleted);
			}

			var allDirectors = await _directorRepository.GetByIdsAsync(newDirectorIds);

			foreach (var directorDto in castDtos)
			{
				var directorId = directorDto.Id!.Value;
				var existingDirector = movie.Directors.FirstOrDefault(x => x.DirectorId == directorId);

				if (existingDirector != null)
				{
					_unitOfWork.Entry(existingDirector, EntityState.Modified);
				}
				else
				{
					var director = allDirectors.FirstOrDefault(p => p.Id == directorId);
					if (director != null)
					{
						var newComboProduct = new MovieDirector
						{
							MovieId = movie.Id,
							DirectorId = director.Id
						};
						_unitOfWork.Entry(newComboProduct, EntityState.Added);
					}
				}
			}
		}
	}
}