using AutoMapper;
using FB98.Modules.Movies.Application.Abstractions;
using FB98.Modules.Movies.Domain.Entities;
using FB98.Shared.Infrastructure.Cloudinaries;
using Microsoft.EntityFrameworkCore;
using StackExchange.Redis;

namespace FB98.Modules.Movies.Application.MovieManagement.Update
{
	internal sealed class UpdateMovieCommandHandler : ICommandHandler<UpdateMovieCommand, ApiResult<object>>
	{
		private readonly ICastRepository _castRepository;
		private readonly ICloudinaryService _cloudinaryService;
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
			ICloudinaryService cloudinaryService,
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
			_cloudinaryService = cloudinaryService;
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
					return ApiResponseBuilder.Error<object>("movie: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
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
					return ApiResponseBuilder.Error<object>("castIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var directorIds = model.Directors.Select(d => d.Id).ToHashSet();
				var existingDirectors = (await _directorRepository.GetAll()
					.Where(d => directorIds.Contains(d.Id))
					.ToListAsync(cancellationToken)).ToDictionary(g => g.Id);
				if (existingDirectors.Count != directorIds.Count)
				{
					return ApiResponseBuilder.Error<object>("directorIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				var genreIds = model.Genres.Select(d => d.Id).ToHashSet();
				var existinggenres = (await _genreRepository.GetAll()
					.Where(d => genreIds.Contains(d.Id))
					.ToListAsync(cancellationToken)).ToDictionary(g => g.Id);
				if (existinggenres.Count != genreIds.Count)
				{
					return ApiResponseBuilder.Error<object>("genreIds: " + _localizedMessageService.GetLocalizedMessage("NotFound"), 404);
				}

				_mapper.Map(model, movie);
				await UpdateGenres(movie, model.Genres);
				await UpdateDirectors(movie, model.Directors);
				await UpdateCasts(movie, model.Casts);

				if (movie.PosterImage != null)
				{
					string? posterImageUrl;
					if (movie.PosterImage != null)
					{
						posterImageUrl = await _cloudinaryService.ReplaceImageAsync(model.PosterImage!, $"movie/{model.Title}");
						movie.PosterImage = posterImageUrl;
					}
					else
					{
						posterImageUrl = await _cloudinaryService.UploadImageAsync(model.PosterImage!, "movie/{model.Title}");
						movie.PosterImage = posterImageUrl;
					}
				}

				if (movie.HeaderImage != null)
				{
					string? headerImageUrl;
					if (movie.HeaderImage != null)
					{
						headerImageUrl = await _cloudinaryService.ReplaceImageAsync(model.HeaderImage!, $"movie/{model.Title}");
						movie.HeaderImage = headerImageUrl;
					}
					else
					{
						headerImageUrl = await _cloudinaryService.UploadImageAsync(model.HeaderImage!, "movie/{model.Title}");
						movie.HeaderImage = headerImageUrl;
					}
				}

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

		private async Task UpdateGenres(Movie movie, IList<UpdateMovieGenreDto> genreDtos)
		{
			var existingGenreIds = movie.Genres.Select(g => g.GenreId).ToList();
			var newGenreIds = genreDtos.Select(g => g.Id!.Value).ToList();

			var genresToRemove = movie.Genres.Where(g => !newGenreIds.Contains(g.GenreId)).ToList();
			foreach (var genre in genresToRemove)
			{
				_unitOfWork.Entry(genre, EntityState.Deleted);
			}

			var genresToAdd = newGenreIds.Except(existingGenreIds).ToList();
			var allGenres = await _genreRepository.GetByIdsAsync(genresToAdd);

			foreach (var genreId in genresToAdd)
			{
				var genre = allGenres.FirstOrDefault(g => g.Id == genreId);
				if (genre != null)
				{
					var newMovieGenre = new MovieGenre
					{
						GenreId = genre.Id,
						MovieId = movie.Id
					};
					_unitOfWork.Entry(newMovieGenre, EntityState.Added);
				}
			}
		}

		private async Task UpdateCasts(Movie movie, IList<UpdateMovieCastDto> castDtos)
		{
			var existingCastIds = movie.Casts.Select(c => c.CastId).ToList();
			var newCastIds = castDtos.Select(c => c.Id!.Value).ToList();

			var castToRemove = movie.Casts.Where(c => !newCastIds.Contains(c.CastId)).ToList();
			foreach (var cast in castToRemove)
			{
				_unitOfWork.Entry(cast, EntityState.Deleted);
			}

			var castMembersToAdd = newCastIds.Except(existingCastIds).ToList();
			var allCastMembers = await _castRepository.GetByIdsAsync(castMembersToAdd);

			foreach (var castId in castMembersToAdd)
			{
				var castMember = allCastMembers.FirstOrDefault(cm => cm.Id == castId);
				if (castMember != null)
				{
					var newMovieCastMember = new MovieCast
					{
						CastId = castMember.Id,
						MovieId = movie.Id
					};
					_unitOfWork.Entry(newMovieCastMember, EntityState.Added);
				}
			}
		}

		private async Task UpdateDirectors(Movie movie, IList<UpdateMovieDirectorDto> castDtos)
		{
			var existingDirectorIds = movie.Directors.Select(c => c.DirectorId).ToList();
			var newDirectorIds = castDtos.Select(c => c.Id!.Value).ToList();

			var castToRemove = movie.Directors.Where(c => !newDirectorIds.Contains(c.DirectorId)).ToList();
			foreach (var cast in castToRemove)
			{
				_unitOfWork.Entry(cast, EntityState.Deleted);
			}

			var castMembersToAdd = newDirectorIds.Except(existingDirectorIds).ToList();
			var allDirectorMembers = await _castRepository.GetByIdsAsync(castMembersToAdd);

			foreach (var castId in castMembersToAdd)
			{
				var castMember = allDirectorMembers.FirstOrDefault(cm => cm.Id == castId);
				if (castMember != null)
				{
					var newMovieDirectorMember = new MovieDirector
					{
						DirectorId = castMember.Id,
						MovieId = movie.Id
					};
					_unitOfWork.Entry(newMovieDirectorMember, EntityState.Added);
				}
			}
		}
	}
}